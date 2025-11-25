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
                    url: 'https://github.com/sakethreddychireddy/Expense_Tracker.git',
                    credentialsId: 'github-creds'
            }
        }

        stage('Restore Dependencies') {
            steps {
                echo '📦 Restoring .NET dependencies...'
                sh 'dotnet restore'
            }
        }

        stage('Build Project') {
            steps {
                echo '🏗️ Building .NET project...'
                sh 'dotnet build --configuration Release'
            }
        }

        stage('Publish Project') {
            steps {
                echo '📤 Publishing .NET project...'
                sh 'dotnet publish -c Release -o ./publish'
            }
        }

        stage('Build Docker Image') {
            steps {
                echo '🐳 Building backend Docker image...'
                script {
                    sh "docker build --network=host -t ${DOCKER_IMAGE}:${BUILD_NUMBER} ."
                    sh "docker tag ${DOCKER_IMAGE}:${BUILD_NUMBER} ${DOCKER_IMAGE}:latest"
                }
            }
        }

        stage('Deploy Container') {
            steps {
                echo '🚀 Deploying backend API container...'
                script {
                    sh '''
                        PORT=5000
                        CONTAINER_NAME=backend_api_container

                        echo "🧭 Checking for existing containers on port $PORT..."
                        docker ps -q --filter "publish=$PORT" | xargs -r docker stop || true
                        docker ps -aq --filter "publish=$PORT" | xargs -r docker rm || true

                        echo "🔍 Checking if container $CONTAINER_NAME exists..."
                        if [ "$(docker ps -aq -f name=$CONTAINER_NAME)" ]; then
                            echo "📦 Stopping existing container..."
                            docker stop $CONTAINER_NAME || true

                            TIMESTAMP=$(date +"%Y%m%d%H%M%S")
                            ARCHIVE_NAME="${CONTAINER_NAME}_${TIMESTAMP}"
                            echo "🗄️ Archiving old container as: $ARCHIVE_NAME"
                            docker rename $CONTAINER_NAME $ARCHIVE_NAME || true
                        fi

                        echo "🧹 Cleaning up dangling Docker images..."
                        docker image prune -f || true

                        echo "🚀 Running new backend API container..."
                        docker run -d -p 5000:80 --name backend_api_container ${DOCKER_IMAGE}:latest

                        echo "✅ Backend deployment complete!"
                    '''
                }
            }
        }
    }

    post {
        success {
            echo "🎉 Backend API deployment successful!"
        }
        failure {
            echo "❌ Backend build or deployment failed. Check Jenkins logs."
        }
    }
}
