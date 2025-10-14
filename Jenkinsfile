pipeline {
    agent any

    environment {
        DOCKERHUB_CREDENTIALS = credentials('dockerhub-credentials')  // add this in Jenkins
        IMAGE_NAME = "expense-tracker/backend"
    }

    stages {
        stage('Checkout') {
            steps {
                echo "🔁 Checking out source code..."
                checkout scm
            }
        }

        stage('Build Docker Images') {
            steps {
                echo "🐋 Building Docker images..."
                sh 'docker compose build'
            }
        }

        stage('Run Containers') {
            steps {
                echo "🚀 Starting containers..."
                sh 'docker compose up -d'
            }
        }

        stage('List Running Containers') {
            steps {
                sh 'docker ps'
            }
        }
    }

    post {
        success {
            echo "✅ Deployment successful for Dev environment!"
        }
        failure {
            echo "❌ Deployment failed! Check logs in Jenkins."
        }
    }
}
