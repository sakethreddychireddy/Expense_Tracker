pipeline {
    agent {
        docker {
            image 'mcr.microsoft.com/dotnet/sdk:8.0'
            args '-v /var/run/docker.sock:/var/run/docker.sock'
        }
    }

    environment {
        APP_NAME = 'expense-tracker-api'
        DOCKER_REGISTRY = 'docker.io/sakethreddychireddy'   // Replace with your Docker Hub username
        DOCKER_CREDENTIALS = 'docker-hub-cred'              // Jenkins credential ID for Docker Hub
    }

    stages {
        stage('Checkout') {
            steps {
                echo '📦 Checking out source code...'
                checkout scm
            }
        }

        stage('Build and Publish') {
            steps {
                echo '⚙️ Building and publishing project...'
                sh 'dotnet restore Expense_Tracker.sln'
                sh 'dotnet publish Expense_Tracker/Expense_Tracker.csproj -c Release -o ./publish'
            }
        }

        stage('Build Docker Image') {
            steps {
                echo '🐳 Building Docker image...'
                script {
                    def buildTag = "dev-${env.BUILD_NUMBER}"
                    def imageName = "${DOCKER_REGISTRY}/${APP_NAME}:${buildTag}"
                    sh "docker build -t ${imageName} -f Expense_Tracker/Dockerfile ."
                    env.DOCKER_IMAGE_NAME = imageName
                }
            }
        }

        stage('Push Docker Image') {
            steps {
                echo '🚀 Pushing Docker image to registry...'
                withCredentials([usernamePassword(credentialsId: "${DOCKER_CREDENTIALS}", usernameVariable: 'DOCKER_USER', passwordVariable: 'DOCKER_PASS')]) {
                    sh 'echo $DOCKER_PASS | docker login -u $DOCKER_USER --password-stdin'
                    sh "docker push ${env.DOCKER_IMAGE_NAME}"
                }
            }
        }

        stage('Deploy (Dev Environment)') {
            steps {
                echo '🔧 Deploying to Docker (Dev Environment)...'
                script {
                    // Stop old containers and run new ones
                    sh """
                        docker-compose down
                        docker-compose pull
                        docker-compose up -d --build
                    """
                }
            }
        }
    }

    post {
        success {
            echo "✅ Build and deploy successful: ${env.DOCKER_IMAGE_NAME}"
        }
        failure {
            echo "❌ Build failed. Check logs."
        }
    }
}
