pipeline {
    agent any // Or a specific agent like 'agent { label 'windows-node' }'

    stages {
        stage('Checkout') {
            steps {
                echo 'Hello Jenkins'
                git 'https://github.com/sakethreddychireddy/Expense_Tracker.git' // Replace with your repository URL
            }
       }
    }
}