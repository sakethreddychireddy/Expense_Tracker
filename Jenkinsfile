pipeline {
    agent any // Or a specific agent like 'agent { label 'windows-node' }'

    stages {
        stage('Checkout') {
            steps {
                git 'https://github.com/sakethreddychireddy/Expense_Tracker.git' // Replace with your repository URL
            }
        }

        stage('Restore NuGet Packages') {
            steps {
                // For .NET Core/.NET 5+
                script {
                    bat 'dotnet restore Expense_Tracker.sln' // Replace with your solution file
                }
                // For .NET Framework (if using nuget.exe directly)
                // bat 'nuget restore YOUR_SOLUTION_FILE.sln'
            }
        }

        stage('Build') {
            steps {
                // For .NET Core/.NET 5+
                script {
                    bat 'dotnet build Expense_Tracker.sln --configuration Release'
                }
                // For .NET Framework (using MSBuild)
                // bat '"C:\\Program Files (x86)\\Microsoft Visual Studio\\2019\\Community\\MSBuild\\Current\\Bin\\MSBuild.exe" YOUR_SOLUTION_FILE.sln /p:Configuration=Release' // Adjust path to MSBuild
            }
        }

        stage('Test') {
            steps {
                // For .NET Core/.NET 5+
                script {
                    bat 'dotnet test Expense_Tracker.sln --configuration Release'
                }
                // For .NET Framework (using VSTest or NUnit/XUnit runners)
                // bat '"C:\\Program Files (x86)\\Microsoft Visual Studio\\2019\\Community\\Common7\\IDE\\CommonExtensions\\Microsoft\\TestWindow\\vstest.console.exe" YourProject.Tests.dll' // Adjust path and test assembly
            }
        }

        stage('Publish/Artifacts') {
            steps {
                // For .NET Core/.NET 5+ to publish for deployment
                script {
                    bat 'dotnet publish Expense_Tracker.csproj --configuration Release --output publish_output'
                }
                archiveArtifacts artifacts: 'publish_output/**' // Archive published output
            }
        }

        // Optional: Deployment stage
        // stage('Deploy') {
        //     steps {
        //         // Add deployment steps here (e.g., to IIS, Azure, etc.)
        //     }
        // }
    }

    post {
        always {
            echo 'Pipeline finished.'
        }
        success {
            echo 'Pipeline succeeded!'
        }
        failure {
            echo 'Pipeline failed!'
        }
    }
}