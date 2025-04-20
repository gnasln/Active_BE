using Bff.Application.Common.Interfaces;
using Bff.Dtos;
using Bff.Identity;
using Bff.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Bff.Application.Tenants.Commands;
using Bff.Application.Dtos.Mail;
using Bff.Application.Contracts.Persistence;
using NetHelper.Common.Models;
using Bff.Application.Tenants.Queries;
using Bff.Domain.Constants;
using Bff.Helper.Services;
using static System.Net.WebRequestMethods;


namespace Bff.Endpoints;

public class User : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(CreateNormalUser, "/create")
            .MapGet(SearchUser, "/{username}")
            .MapPost(CheckEmails, "/check-email")
            .MapPost(CheckOtps, "/check-otp")
            .MapPost(UpdatePassword, "forgot-pw")
            ;
        app.MapGroup(this)
            .RequireAuthorization()
            .MapPost(ChangePassword, "/change-password")
            .MapPost(CreateNewUserFromOwnerOfTenant, "/create-member-and-add-to-tenant")
            .MapPatch(UpdateUser, "/update-user")
            .MapGet(GetListUserId, "/get-user-id")
            .MapGet(GetAllUserTenant, "/get-all-user-tenant")
            .MapPatch(UpdateUserByAdmin, "/update-user-by-admin");
        app.MapGroup(this)
            .RequireAuthorization("admin")
            .MapGet(GetAllUser, "/get-all-user");
    }

    public async Task<IResult> CreateNormalUser(UserManager<ApplicationUser> _userManager, [FromBody] NewUser newUser)
    {
        if (!newUser.Password.Equals(newUser.Repassword))
        {
            return Results.BadRequest("500|Password and RePassword not same");
        }

        var User = await _userManager.FindByNameAsync(newUser.UserName);
        if (User is not null)
        {
            return Results.BadRequest("501|User existed");
        }

        var user = new ApplicationUser { UserName = newUser.UserName, Email = newUser.Email, FullName = newUser.FullName, PhoneNumber = newUser.PhoneNumber, ActivationDate = newUser.ActivationDate };
        var ret = await _userManager.CreateAsync(user, newUser.Password);

        if (ret.Succeeded)
        {
            var roleResult = await _userManager.AddToRoleAsync(user, Roles.User);
            if (!roleResult.Succeeded)
            {
                return Results.BadRequest("500|Add role failed");
            }
            return Results.Ok("200|User created");
        }
        else
        {
            return Results.BadRequest($"500|{string.Concat(ret.Errors.Select(e => e.Description))}");
        }
    }

    /* forgot password */
    // checkemail
    public async Task<IResult> CheckEmails([FromServices] OTPService OtpService, UserManager<ApplicationUser> UserManager, [FromBody] CheckEmail checkEmail,
        [FromServices] IMailServices MailServices)
    {
        try
        {
            var user = UserManager.Users.FirstOrDefault(u => u.Email == checkEmail.Email);
            if (user == null)
            {
                return Results.NotFound("User not found.");
            }
            //tao otp
            var verifyCodeOtp = OtpService.GenerateOTP();
            OtpService.SaveOTP(checkEmail.Email, verifyCodeOtp);

            await MailServices.SendEmailAsync(checkEmail.Email, user.FullName!, verifyCodeOtp);
            //luu otp vao db
            user.OTP = verifyCodeOtp;
            user.Expires_at = DateTime.UtcNow.AddMinutes(10);
            user.Updated_at = DateTime.UtcNow;

            var result = await UserManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Results.Ok(new { message = "200|User updated successfully", username = user.UserName });
            }
            else
            {
                return Results.BadRequest($"500|{string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
        catch (Exception e)
        {
            return Results.BadRequest(e);
        }
    }

    private string GenerateVerificationCode(int length = 6)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    //check otp
    public async Task<IResult> CheckOtps(UserManager<ApplicationUser> _userManager, [FromBody] CheckOtp checkOtp)
    {
        try
        {
            var user = _userManager.Users.FirstOrDefault(u => u.Email == checkOtp.Email);
            if (user == null)
            {
                return Results.NotFound("User not found.");
            }

            if (checkOtp.OTP != user.OTP || checkOtp.Email != user.Email)
            {
                return Results.BadRequest("400 | thong tin cua otp or Email khong hop le !");
            }

            if (user.Expires_at < DateTime.UtcNow)
            {
                return Results.BadRequest("400 | otp da het hieu luc !");
            }

            return Results.Ok(new { message = "check otp thanh cong" });
        }
        catch (System.Exception)
        {
            return Results.BadRequest("500 | error check otp");
        }
    }

    //update password
    public async Task<IResult> UpdatePassword(UserManager<ApplicationUser> _userManager,
        [FromBody] ForgotPassword updatePassword)
    {
        try
        {
            var user = await _userManager.FindByNameAsync(updatePassword.UserName);
            if (user is null)
            {
                return Results.BadRequest("400 | user not found !");
            }

            if (updatePassword.new_password != updatePassword.confirmed_password)
            {
                return Results.BadRequest("400 | password not match !");
            }

            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, updatePassword.new_password);
            await _userManager.UpdateAsync(user);
            user.OTP = null;
            user.Expires_at = null;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Results.Ok(new { message = "update password thanh cong" });
            }
            else
            {
                return Results.BadRequest($"500|{string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
        catch (System.Exception)
        {
            return Results.BadRequest("500 | error forgot password !");
        }
    }

    /*
    C**** : list User Id
    */
    public async Task<IResult> GetListUserId(UserManager<ApplicationUser> _userManager, [FromServices] IUser _user)
    {
        try
        {
            var userId = _user.Id;
            if (string.IsNullOrEmpty(userId))
            {
                return Results.BadRequest("400 | user is not found !");
            }

            var user = await _userManager.FindByIdAsync(userId);
            var userDetail = new
            {
                Email = user.Email,
                UserName = user.UserName,
                Birthday = user.Birthday,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber
            };
            return Results.Ok(new { status = 200, message = "get user successfully!", data = userDetail });
        }
        catch (System.Exception)
        {
            return Results.BadRequest("500| error list user id");
        }
    }

    /*
    C****: getalluser
    */
    public async Task<ResultCustomPaginate<IEnumerable<UserDto>>> GetAllUser(UserManager<ApplicationUser> _userManager, int page, int pageSize)
    {
        var usersQuery = _userManager.Users;

        var usersList = await usersQuery.ToListAsync();

        var usersDtoList = new List<UserDto>();

        foreach (var u in usersList)
        {
            // Lấy vai trò của người dùng
            var roles = await _userManager.GetRolesAsync(u);
            var roleUser = roles.FirstOrDefault();
            var isLockedAccount = await IsAccountLockedAsync(_userManager, u.Id.ToString());

            // Tạo đối tượng UserDto
            var userDto = new UserDto
            {
                Id = Guid.Parse(u.Id),
                UserName = u.UserName,
                Email = u.Email,
                NumberPhone = u.PhoneNumber,
                TenantName = u.Tenant,
                status = isLockedAccount ? "Không hoạt động" : "Đang hoạt động",
                RoleUser = roleUser,
                ActivationDate = u.ActivationDate // Ensure this property exists in UserDto
            };

            usersDtoList.Add(userDto);
        }

        // Sort by ActivationDate
        var sortedUsersDtoList = usersDtoList.OrderByDescending(u => u.ActivationDate).ToList();

        // Áp dụng phân trang
        var paginatedUsers = sortedUsersDtoList.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        var result = new ResultCustomPaginate<IEnumerable<UserDto>>
        {
            Data = paginatedUsers,
            PageNumber = page,
            PageSize = pageSize,
            TotalItems = sortedUsersDtoList.Count,
            TotalPages = (int)Math.Ceiling((double)sortedUsersDtoList.Count / pageSize)
        };

        return result;
    }


    /*
    C****: getAllUserTenant
    */

    public async Task<ResultCustomPaginate<IEnumerable<UserDto>>> GetAllUserTenant(UserManager<ApplicationUser> _userManager, [FromServices] ISender _sender, IApplicationDbContext _context, IUser _user, int page, int pageSize)
    {
        // Get tenant of user
        var myTenant = await _context.Tenants.Where(t => t.Owner == Guid.Parse(_user.Id!)).ToListAsync();

        // Collect all users from the tenants
        var usersQuery = _userManager.Users.Where(u => myTenant.Select(t => t.Name).Contains(u.Tenant));

        var usersList = await usersQuery.ToListAsync();

        var usersDtoList = new List<UserDto>();

        foreach (var u in usersList)
        {
            // Lấy vai trò của người dùng
            var roles = await _userManager.GetRolesAsync(u);
            var roleUser = roles.FirstOrDefault();
            var isLockedAccount = await IsAccountLockedAsync(_userManager, u.Id.ToString());

            // Tạo đối tượng UserDto
            var userDto = new UserDto
            {
                Id = Guid.Parse(u.Id),
                UserName = u.UserName,
                Email = u.Email,
                NumberPhone = u.PhoneNumber,
                TenantName = u.Tenant,
                status = isLockedAccount ? "Không hoạt động" : "Đang hoạt động",
                RoleUser = roleUser,
                ActivationDate = u.ActivationDate // Ensure this property exists in UserDto
            };

            usersDtoList.Add(userDto);
        }

        // Sort by ActivationDate in descending order
        var sortedUsersDtoList = usersDtoList.OrderByDescending(u => u.ActivationDate).ToList();

        // Apply pagination
        var paginatedUsers = sortedUsersDtoList.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        var result = new ResultCustomPaginate<IEnumerable<UserDto>>
        {
            Data = paginatedUsers,
            PageNumber = page,
            PageSize = pageSize,
            TotalItems = sortedUsersDtoList.Count,
            TotalPages = (int)Math.Ceiling((double)sortedUsersDtoList.Count / pageSize)
        };

        return result;
    }

    private async Task<bool> IsAccountLockedAsync(UserManager<ApplicationUser> _userManager, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        var expiresAt = user.Expires_at;
        if (expiresAt.HasValue && expiresAt < DateTimeOffset.Now)
        {
            return true;
        }

        return false;
    }

    /*
    C****: Update User
    */
    public async Task<IResult> UpdateUser(UserManager<ApplicationUser> _userManager, [FromBody] UpdateUser _userInit,
        [FromServices] IUser _user)
    {
        try
        {
            var userId = _user.Id;
            if (string.IsNullOrEmpty(userId))
            {
                return Results.BadRequest("400|UserId is empty");
            }

            var currentUser = await _userManager.FindByIdAsync(userId);
            if (currentUser == null)
            {
                return Results.BadRequest("400|UserId không hợp lệ !");
            }

            if(_userInit.UserName != null) currentUser.UserName = _userInit.UserName;
            if(_userInit.Birthday != null) currentUser.Birthday = _userInit.Birthday;
            if(_userInit.Email != null) currentUser.Email = _userInit.Email;
            if(_userInit.Phone != null) currentUser.PhoneNumber = _userInit.Phone;
            if(_userInit.Address != null) currentUser.Address = _userInit.Address;
            if(_userInit.Status != null) currentUser.Status = _userInit.Status;
            if(_userInit.FullName != null) currentUser.FullName = _userInit.FullName;
            
            var result = await _userManager.UpdateAsync(currentUser);
            if (result.Succeeded)
            {
                return Results.Ok("200|User updated successfully");
            }
            else
            {
                return Results.BadRequest($"500|{string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("error", ex.Message);
            return Results.Problem("An error occurred while updating the user", statusCode: 500);
        }
    }

    /*
    C****: Change Password
    */

    public async Task<IResult> ChangePassword(UserManager<ApplicationUser> _userManager,
        [FromBody] ChangePassword changePassword, IUser _user)
    {
        try
        {
            var userId = _user.Id;
            if (string.IsNullOrEmpty(userId))
            {
                return Results.BadRequest("400| UserId is empty");
            }

            var currentUser = await _userManager.FindByIdAsync(userId);
            if (currentUser == null)
            {
                return Results.BadRequest("400|UserId không hợp lệ !");
            }

            var isOldPasswordCorrect = await _userManager.CheckPasswordAsync(currentUser, changePassword.oldPassword);
            if (!isOldPasswordCorrect)
            {
                return Results.BadRequest("400|Old password is incorrect");
            }

            if (!changePassword.newPassword.Equals(changePassword.comfirmedPassword))
            {
                return Results.BadRequest("400|mật khẩu mới không trùng hợp !");
            }

            var result = await _userManager.ChangePasswordAsync(currentUser, changePassword.oldPassword,
                changePassword.newPassword);
            if (result.Succeeded)
            {
                return Results.Ok("200|Password changed successfully");
            }
            else
            {
                return Results.BadRequest($"500|{string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("error", ex.Message);
            return Results.Problem("An error occurred while changing the password", statusCode: 500);
        }
    }

    public async Task<IResult> SearchUser(UserManager<ApplicationUser> _userManager, [FromRoute] string username)
    {
        try
        {
            var users = await _userManager.Users
                .Where(u => u.NormalizedUserName.Contains(username.ToUpper()))
                .Select(u => new SimpleUserInfo
                {
                    UserName = u.UserName ?? "",
                    UserId = Guid.Parse(u.Id),
                    FullName = u.FullName ?? ""
                }).ToListAsync();

            if (users.Any())
            {
                return Results.Ok(new
                {
                    status = 200,
                    message = "get users successfully!",
                    data = users
                });
            }

            return Results.NotFound(new
            {
                status = 404,
                message = "can not find users"
            });
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new
            {
                status = 500,
                message = $"ERROR :: {ex}"
            });
        }
    }


    // tạo mới người dùng từ phía owner , nếu thành công thì cho người dùng đó vào tổ chức mà họ muốn 

    public async Task<IResult> CreateNewUserFromOwnerOfTenant(
        UserManager<ApplicationUser> _userManager,
        IApplicationDbContext _db,
        IUser _user,
        [FromBody] NewUserFromOwnerTenant newUser,
        [FromServices] IMediator _mediator
    )
    {
        try
        {
            // check pw và rpw
            if (!newUser.Password.Equals(newUser.Repassword))
            {
                return Results.BadRequest(new
                {
                    status = 400,
                    message = "password and repassword is not same"
                });
            }

            // tìm xem người dùng đã tồn tại hay chưa ? 
            var User = await _userManager.FindByNameAsync(newUser.UserName);
            if (User is not null)
            {
                return Results.BadRequest(new
                {
                    status = 400,
                    message = "UserName existed!"
                });
            }

            // kiểm tra người thực hiện có phải chủ nhân của tenant hay không 
            var checkOwnerTenant = await _db.Tenants
                .Where(t => t.Id == newUser.TenantId && t.Owner == Guid.Parse(_user.Id))
                .FirstOrDefaultAsync();
            if (checkOwnerTenant is null)
                return Results.BadRequest(new
                {
                    status = 403,
                    message = "forbidden"
                });

            // tạo mới user 
            var user = new ApplicationUser { UserName = newUser.UserName };
            var res = await _userManager.CreateAsync(user, newUser.Password);

            if (res.Succeeded)
            {
                // thêm role cho user
                var roleResult = await _userManager.AddToRoleAsync(user, Roles.User);
                if (!roleResult.Succeeded)
                {
                    return Results.BadRequest(new
                    {
                        status = 400,
                        message = "add role failed"
                    });
                }
                var newMember = await _userManager.FindByNameAsync(newUser.UserName);
                // thêm người dùng vào tổ chức
                AddMemberToTenantCommand addMember = new()
                {
                    UserId = Guid.Parse(newMember.Id),
                    UserName = newMember.UserName,
                    TenantId = newUser.TenantId
                };
                await _mediator.Send(addMember);

                return Results.Ok(new
                {
                    status = 201,
                    message = $"create new user and add tenant id ={newUser.TenantId} successfully !"
                });
            }

            return Results.BadRequest(new
            {
                status = 400,
                message = "create new user failed, try again ...."
            });
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new
            {
                status = 500,
                message = $"ERROR :: {ex}"
            });
        }
    }

    public async Task<IResult> UpdateUserByAdmin(UserManager<ApplicationUser> _userManager, [FromBody] UserDto userDto, [FromServices] ISender _sender, ApplicationDbContext _context)
    {
        var user = await _userManager.FindByIdAsync(userDto.Id.ToString());
        if (user is null)
        {
            return Results.NotFound(new
            {
                status = 404,
                message = "User not found"
            });
        }
        if (userDto.UserName is not null) user.UserName = userDto.UserName;
        if (userDto.Email is not null) user.Email = userDto.Email;
        if (userDto.NumberPhone is not null) user.PhoneNumber = userDto.NumberPhone;
        if (userDto.TenantName is not null)
        {
            var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Name == userDto.TenantName);
            await _sender.Send(new DeleteMemberCommand() { TenantId = tenant!.Id, UserId = userDto.Id });
            user.Tenant = userDto.TenantName;
            var newTenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Name == userDto.TenantName);
            if (newTenant is not null)
            {
                await _sender.Send(new AddMemberToTenantCommand() { TenantId = newTenant.Id, UserId = userDto.Id, UserName = userDto.UserName });
            }
        }
        if (userDto.RoleUser is not null)
        {
            var roles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roles);
            await _userManager.AddToRoleAsync(user, userDto.RoleUser);
        }
        await _userManager.UpdateAsync(user);
        return Results.Ok(new
        {
            status = 200,
            message = "Update user successfully"
        });
    }

}