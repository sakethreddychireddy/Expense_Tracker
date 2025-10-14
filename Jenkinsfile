pipeline {
    agent any

    stages {
        stage('Hello') {
            steps {
                echo 'Hello World'
            }
        }
        stage('Checkout') {
            steps {
                git branch: 'main', credentialsId: '324a389c-02aa-4f98-a130-cee8df9da58a', url: 'https://github.com/sakethreddychireddy/Expense_Tracker.git'
            }
        }
        stage('Restore NuGet Packages') {
            steps {
                // For .NET Core/.NET 5+
                script {
                     //sh 'pwd'
                     //sh 'ls'
                     sh 'dotnet restore Expense_Tracker.csproj' // Replace with your solution file
                }
                // For .NET Framework (if using nuget.exe directly)
                // bat 'nuget restore YOUR_SOLUTION_FILE.sln'
            }
        }
         stage('Build') {
            steps {
                // For .NET Core/.NET 5+
                script {
                    sh 'dotnet build Expense_Tracker.csproj --configuration Release'
                }
                // For .NET Framework (using MSBuild)
                // bat '"C:\\Program Files (x86)\\Microsoft Visual Studio\\2019\\Community\\MSBuild\\Current\\Bin\\MSBuild.exe" YOUR_SOLUTION_FILE.sln /p:Configuration=Release' // Adjust path to MSBuild
            }
        }
        stage('Publish/Artifacts') {
            steps {
                // For .NET Core/.NET 5+ to publish for deployment
                script {
                    sh 'rm -rf publish'
                    sh 'dotnet publish Expense_Tracker.csproj --configuration Release --output publish'
                    //sh '  whoami'
                    //sh 'cp -r publish/ /home/saketh/Expense_Tracker/publish'
                }
               // archiveArtifacts artifacts: 'publish_output/**' // Archive published output
            }
        }
    }
}