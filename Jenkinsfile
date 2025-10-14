pipeline {
    agent any

    environment {
        APP_NAME = 'expense_tracker-backend'
        DOCKER_IMAGE = 'expense_tracker-backend'
        DOCKER_CONTAINER = 'expense_tracker_api'
        BRANCH_NAME = 'main'
    }

    stages {
        stage('Checkout') {
            steps {
                echo '📥 Cloning repository...'
                git branch: "${BRANCH_NAME}",
                    url: 'https://github.com/sakethreddychireddy/Expense_Tracker.git'
            }
        }

        stage('Restore Dependencies') {
            steps {
                echo '⚙️ Restoring .NET dependencies...'
                sh 'dotnet restore'
            }
        }

        stage('Build Project') {
            steps {
                echo '🏗️ Building ASP.NET Core project...'
                sh 'dotnet build --configuration Release'
            }
        }

        stage('Publish Project') {
            steps {
                echo '📦 Publishing application...'
                sh 'dotnet publish -c Release -o ./publish'
            }
        }

        stage('Build Docker Image') {
            steps {
                echo '🐳 Building Docker image...'
                sh "docker build -t ${DOCKER_IMAGE}:latest ."
            }
        }

        stage('Deploy Container') {
            steps {
                echo '🚀 Deploying Docker container...'
                script {
                    sh '''
                        # Stop and remove any existing container
                        docker ps -q --filter "name=expense_tracker_api" | grep -q . && \
                        docker stop expense_tracker_api && docker rm expense_tracker_api || true

                        # Run the new container
                        docker run -d -p 8080:8080 --name expense_tracker_api expense_tracker-backend:latest
                    '''
                }
            }
        }
    }

    post {
        success {
            echo "✅ Deployment successful!"
        }
        failure {
            echo "❌ Build or deployment failed. Check Jenkins logs for details."
        }
    }
}
