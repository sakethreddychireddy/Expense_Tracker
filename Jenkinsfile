pipeline {
    agent any

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Build Docker Image') {
            steps {
                echo "Building Docker image for ASP.NET Core Web API..."
                sh 'docker compose build'
            }
        }

        stage('Run Containers') {
            steps {
                echo "Starting containers..."
                sh 'docker compose up -d'
            }
        }

        stage('Verify Containers') {
            steps {
                sh 'docker ps'
            }
        }
    }

    post {
        success {
            echo "✅ Dev environment started successfully!"
        }
        failure {
            echo "❌ Pipeline failed — check the Jenkins logs."
        }
    }
}
