pipeline {
    agent any

    environment {
        DOCKER_IMAGE = 'aspnet-backend:latest' // Docker image name
        REGISTRY = '' // Set Docker Hub username if pushing
        APP_PORT = '5000' // Port to expose on host
    }

    stages {
        stage('Checkout') {
            steps {
                echo 'Checking out code...'
                checkout scm
            }
        }

        stage('Restore Dependencies') {
            steps {
                echo 'Restoring .NET dependencies...'
                sh 'dotnet restore'
            }
        }

        stage('Build') {
            steps {
                echo 'Building application...'
                sh 'dotnet build --configuration Release'
            }
        }

        stage('Test') {
            steps {
                echo 'Running unit tests...'
                sh 'dotnet test --no-build --configuration Release'
            }
        }

        stage('Publish') {
            steps {
                echo 'Publishing application...'
                sh 'dotnet publish -c Release -o ./publish'
            }
        }

        stage('Build Docker Image') {
            steps {
                echo 'Building Docker image...'
                sh "docker build -t ${DOCKER_IMAGE} ."
            }
        }

        stage('Push Docker Image') {
            when {
                expression { return env.REGISTRY != '' }
            }
            steps {
                echo 'Pushing Docker image to registry...'
                sh "docker tag ${DOCKER_IMAGE} ${REGISTRY}/${DOCKER_IMAGE}"
                sh "docker push ${REGISTRY}/${DOCKER_IMAGE}"
            }
        }

        stage('Deploy') {
            steps {
                echo 'Deploying application...'
                sh """
                docker stop aspnet_backend || true
                docker rm aspnet_backend || true
                docker run -d -p ${APP_PORT}:80 --name aspnet_backend ${DOCKER_IMAGE}
                """
            }
        }
    }

    post {
        success {
            echo 'Deployment completed successfully!'
        }
        failure {
            echo 'Deployment failed.'
        }
    }
}
