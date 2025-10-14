pipeline {
    agent any

    environment {
        APP_NAME = 'expense-tracker'
        DOCKER_REGISTRY = 'docker.io/sakethreddychireddy'
        DOCKER_CREDENTIALS = 'docker-hub-cred'
    }

    stages {
        stage('Checkout') {
            steps {
                echo 'Checking out source code...'
                checkout scm
            }
        }

        stage('Build & Publish') {
            steps {
                echo 'Building the .NET 8 Web API...'
                sh 'dotnet restore'
                sh 'dotnet publish -c Release -o ./publish'
            }
        }

        stage('Build Docker Image') {
            steps {
                echo 'Building Docker image...'
                script {
                    def buildNumber = env.BUILD_NUMBER
                    def dockerImage = "${DOCKER_REGISTRY}/${APP_NAME}:${buildNumber}"
                    sh "docker build -t ${dockerImage} -f Dockerfile ."
                    env.DOCKER_IMAGE_NAME = dockerImage
                }
            }
        }

        stage('Push Docker Image') {
            steps {
                echo 'Pushing Docker image to registry...'
                withCredentials([usernamePassword(credentialsId: DOCKER_CREDENTIALS, usernameVariable: 'DOCKER_USERNAME', passwordVariable: 'DOCKER_PASSWORD')]) {
                    sh "docker login -u ${DOCKER_USERNAME} -p ${DOCKER_PASSWORD}"
                    sh "docker push ${env.DOCKER_IMAGE_NAME}"
                }
            }
        }

        stage('Deploy (Docker Compose)') {
            steps {
                echo 'Deploying application with docker-compose...'
                sh 'docker compose down || true'
                sh 'docker compose up -d --build'
            }
        }
    }

    post {
        always {
            echo 'Pipeline finished.'
        }
        success {
            echo '✅ Application deployed successfully.'
        }
        failure {
            echo '❌ Pipeline failed.'
        }
    }
}
