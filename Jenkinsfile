pipeline {
    agent any

    environment {
        DOCKER_IMAGE = 'expense_tracker/backend-api'
        BRANCH_NAME = 'main'
    }

    stages {
        stage('Checkout') {
            steps {
                echo '📥 Cloning backend repository...'
                git branch: "${BRANCH_NAME}",
                    url: 'https://github.com/sakethreddychireddy/Expense_Tracker.git'
            }
        }

        stage('Restore & Build') {
            steps {
                echo '🧱 Building ASP.NET Core project...'
                sh 'dotnet restore'
                sh 'dotnet build --configuration Release'
            }
        }

        stage('Publish') {
            steps {
                echo '📦 Publishing for Docker...'
                sh 'dotnet publish -c Release -o out'
            }
        }

        stage('Build Docker Image') {
            steps {
                echo '🐳 Building Docker image...'
                script {
                    sh "docker build -t ${DOCKER_IMAGE}:${BUILD_NUMBER} ."
                    sh "docker tag ${DOCKER_IMAGE}:${BUILD_NUMBER} ${DOCKER_IMAGE}:latest"
                }
            }
        }

        stage('Deploy Container') {
            steps {
                echo '🚀 Deploying container...'
                script {
                    sh '''
                        docker ps -q --filter "name=expense_api_container" | grep -q . && \
                        docker stop expense_api_container && docker rm expense_api_container || true

                        docker run -d -p 8081:8081 --name expense_api_container ${DOCKER_IMAGE}:latest
                    '''
                }
            }
        }
    }

    post {
        success {
            echo "✅ Backend API deployed successfully at http://server:8081"
        }
        failure {
            echo "❌ Backend build or deployment failed. Check Jenkins logs."
        }
    }
}
