pipeline {
		agent any

		environment {
			gitRepository_source = 'https://gitlab.eztek.net/active/activebff.git'
			gitBranch_source = 'develop'
			gitlabCredential = 'gitlab_jenkin'

			gitRepository_manifest = 'http://gitlab.eztek.net/kubernetes/manifest.git'
			gitBranch_manifest = 'main'

			dockerregistry = 'https://nexus.eztek.net'
			registryCredential = 'nexus_jenkin'

			dockerimagename = "nexus.eztek.net/active/active-api"
			dockerimage_tag = "nexus.eztek.net/${dockerimagename}"
			version = "stable-0.${BUILD_NUMBER}"
			DOCKERFILE = "ActiveBff/Dockerfile"
			Context = "ActiveBff"
		}

		stages {
			stage('Checkout project')
			{
			  steps
			  {
				git branch: gitBranch_source,
				   credentialsId: gitlabCredential,
				   url: gitRepository_source
				sh "git reset --hard"
			  }
			}
			stage('Build docker')
			{
			    agent any
			  steps
			  {
				script {
                        sh "pwd"
						dockerImage = docker.build (dockerimagename, "-f \$DOCKERFILE $Context")
					}
			  }
			}
			stage('Pushing Image')
			{
      		  steps
			  {
        		script {
           		 docker.withRegistry( dockerregistry , registryCredential ) {
              	 dockerImage.push(version)
				 sh "docker rmi ${dockerimagename}:${version} -f"
                 sh "docker rmi ${dockerimagename}:lastest -f"
           		 }
          		}
        	  }
    	    }
			stage('Update Manifest File')
			{
      		  steps
			  {
				git branch: gitBranch_manifest,
				   credentialsId: gitlabCredential,
				   url: gitRepository_manifest
				sh "git reset --hard"
				script {
                        sh "pwd"
						sh "sed -i 's+${dockerimagename}.*+${dockerimagename}:${version}+g' Active/Active-api/Deployment.yaml"
						sh "git add ."
						sh "git commit -m '[update] manifest file tag image'"
						sh "git push origin main"
					}
			  }
            }
		}
	}
