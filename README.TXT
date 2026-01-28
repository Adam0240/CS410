Project: zuul-bad
Authors: Michael Kölling and David J. Barnes

Modified by Adam 1/14/26
linked to jira
test 5

This project is part of the material for the book

   Objects First with Java - A Practical Introduction using BlueJ
   Sixth edition
   David J. Barnes and Michael Kölling
   Pearson Education, 2016

This project is a simple framework for an adventure game. In this version,
it has a few rooms and the ability for a player to walk between these rooms.
That's all.

To start this application, create an instance of class "Game" and call its
"play" method.

This version of the game contains some very bad class design. It should NOT
be used as a basis for extending the project without fixing these design
problems. It serves as an example to discuss good and bad design (chapter 8
of the book).

pipeline {
    agent any

    parameters {
        string(name: 'GIT_BRANCH', defaultValue: 'main', description: 'Branch to build')
        string(name: 'GIT_REPO', defaultValue: 'git@github.com:WVUP/MobileAPI.git', description: 'Git repo SSH URL')
        string(name: 'CREDENTIALS', defaultValue: 'ssh_key', description: 'Jenkins Credentials ID')
    }

    stages {
        stage('Checkout Source') {
            steps {
                git branch: "${params.GIT_BRANCH}",
                    url: "${params.GIT_REPO}",
                    credentialsId: "${params.CREDENTIALS}"
            }
        }
    }

    post {
        success {
            echo "✅ Checkout successful."
        }
        failure {
            echo "❌ Checkout failed."
        }
    }
}

Chapter 8 of the book contains a detailed description of the problems in this
project, and how to fix them.

The project 'zuul-better' contains a version of this project with better
designed class structure. It includes the fixes discussed in the book.
