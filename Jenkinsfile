pipeline {
    agent any

    environment {
        APP_NAME = 'expense_tracker_api'
        DOCKER_COMPOSE_FILE = 'docker-compose.yml'
        BUILD_DIR = 'publish'
        ARCHIVE_DIR = '/var/jenkins_home/build_archives'
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

        stage('Build .NET Project') {
            steps {
                echo '⚙️ Building ASP.NET 8 project...'
                sh 'dotnet build Expense_Tracker/Expense_Tracker.csproj -c Release'
            }
        }

        stage('Publish .NET Project') {
            steps {
                echo '📦 Publishing ASP.NET 8 project...'
                sh 'dotnet publish Expense_Tracker/Expense_Tracker.csproj -c Release -o publish'
            }
        }

        stage('Archive Old Build') {
            steps {
                echo '🗄️ Archiving previous build...'
                sh '''
                    mkdir -p ${ARCHIVE_DIR}
                    TIMESTAMP=$(date +%Y%m%d_%H%M%S)
                    if [ -d "${BUILD_DIR}" ]; then
                        tar -czf ${ARCHIVE_DIR}/${APP_NAME}_$TIMESTAMP.tar.gz ${BUILD_DIR} || true
                        echo "✅ Old build archived as ${ARCHIVE_DIR}/${APP_NAME}_$TIMESTAMP.tar.gz"
                    else
                        echo "⚠️ No previous build found to archive."
                    fi
                '''
            }
        }

        stage('Build Docker Image') {
            steps {
                echo '🐳 Building Docker image...'
                sh "docker compose -f ${DOCKER_COMPOSE_FILE} build"
            }
        }

        stage('Deploy Containers') {
            steps {
                echo '🚀 Deploying containers...'
                sh "docker compose -f ${DOCKER_COMPOSE_FILE} down"
                sh "docker compose -f ${DOCKER_COMPOSE_FILE} up -d"
            }
        }
    }

    post {
        success {
            echo '✅ Deployment completed successfully!'
        }
        failure {
            echo '❌ Deployment failed.'
        }
    }
}
