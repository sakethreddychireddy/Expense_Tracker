pipeline {
    agent any
    environment {
        APP_NAME = 'expense_tracker_api'
        BUILD_DIR = 'publish'
        ARCHIVE_DIR = '/var/jenkins_home/build_archives'
        DOCKER_COMPOSE_FILE = 'docker-compose.yml'
        BRANCH_NAME = 'main'
    }
    stages {
        stage('Checkout') {
            steps {
                echo '🔹 Checking out code...'
                git branch: "${BRANCH_NAME}",
                    url: 'https://github.com/sakethreddychireddy/Expense_Tracker.git'
            }
        }

        stage('Build .NET Project') {
            steps {
                echo '🔹 Building the .NET project...'
                sh 'dotnet build Expense_Tracker/Expense_Tracker.csproj -c Release'
            }
        }

        stage('Publish .NET Project') {
            steps {
                echo '🔹 Publishing the project...'
                sh 'dotnet publish Expense_Tracker/Expense_Tracker.csproj -c Release -o ${BUILD_DIR}'
            }
        }

        stage('Archive Old Build') {
            steps {
                echo '🔹 Archiving old build (if exists)...'
                sh '''
                    mkdir -p ${ARCHIVE_DIR}
                    TIMESTAMP=$(date +%Y%m%d%H%M%S)
                    if [ -d "${BUILD_DIR}" ]; then
                        mv ${BUILD_DIR} ${ARCHIVE_DIR}/${APP_NAME}_${TIMESTAMP} || true
                    fi
                '''
            }
        }

        stage('Build Docker Image') {
            steps {
                echo '🐳 Building Docker image...'
                sh "docker-compose -f ${DOCKER_COMPOSE_FILE} build"
            }
        }

        stage('Deploy with Docker Compose') {
            steps {
                echo '🚀 Deploying containers...'
                sh """
                    docker-compose -f ${DOCKER_COMPOSE_FILE} down
                    docker-compose -f ${DOCKER_COMPOSE_FILE} up -d
                """
            }
        }
    }

    post {
        success {
            echo '✅ Pipeline completed successfully!'
        }
        failure {
            echo '❌ Pipeline failed. Check Jenkins logs for details.'
        }
        always {
            echo '🧹 Cleaning up workspace...'
            cleanWs()
        }
    }
}
