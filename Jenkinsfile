pipeline {
	agent any
	environment {
		// Define environment variables
		APP_NAME = 'expense-tracker-api'
		DOCKER_COMPOSE_FILE = 'docker-compose.yml'
		BUILD_DIR = 'publish'
		ARCHIVE_DIR = '/var/jenkins_home/build_archives'
		BRANCH_NAME = 'main'
		}
		stages {
			stage('Checkout') {
				steps {
					// Checkout the code from the repository
					echo 'Checking out code...'
					git branch: "${BRANCH_NAME}",
					url: 'https://github.com/sakethreddychireddy/Expense_Tracker.git'
					}
			}
			stage('Build .NET Project') {
				steps {
					// Build the application
					echo 'Building the application...'
					sh 'dotnet build Expense_Tracker/Expense_Tracker.csproj -c Release'
					}
			}
			stage('Publish .NET Project') {
				steps {
					// Publish the application to a specified directory
					echo 'Publishing the application...'
					sh 'dotnet publish Expense_Tracker/Expense_Tracker.csproj -c Release -o publish'
					}
			}
			stage('Archive Old Build') {
				steps {
					// Archive the old build if it exists
					echo 'Archiving old build if it exists...'
					sh '''
					mkdir -p ${ARCHIVE_DIR}
					TIMESTAMP=$(date +%Y%m%d%H%M%S)
					if [ -d "${BUILD_DIR}" ]; then
						mv ${BUILD_DIR} ${ARCHIVE_DIR}/${APP_NAME}_$TIMESTAMP
					fi
					'''
				}
			}
			stage('Build Docker Image') {
				steps {
					// Build the Docker image using Docker Compose
					echo 'Building Docker image...'
					sh 'docker-compose -f ${DOCKER_COMPOSE_FILE} build'
					}
			}
			stage('Docker Containers Up') {
				steps {
					// Start the Docker containers using Docker Compose
					echo 'Starting Docker containers...'
					sh 'docker-compose -f ${DOCKER_COMPOSE_FILE} down'
					sh 'docker-compose -f ${DOCKER_COMPOSE_FILE} up -d'
					}
			}
		}
		post {
			success {
				echo 'Pipeline completed successfully!'
			}
			failure {
				echo 'Pipeline failed. Please check the logs.'
			}
		}
}