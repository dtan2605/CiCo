pipeline {
    agent any

    environment {
        DOCKER_BUILDKIT = "1"
        NEXT_TELEMETRY_DISABLED = "1"
    }

    stages {
        stage('Build API Image') {
            steps {
                dir('/workspace') {
                    script {
                        docker.build(
                            "cico-api:latest",
                            "-f cico.API/Dockerfile ."
                        )
                    }
                }
            }
        }

        stage('Build Web Image') {
            steps {
                dir('/workspace') {
                    script {
                        docker.build(
                            "cico-web:latest",
                            "-f cico-web/Dockerfile cico-web"
                        )
                    }
                }
            }
        }

        stage('Deploy') {
            steps {
                dir('/workspace') {
                    script {
                        sh 'docker compose -f docker-compose.yml up -d'
                    }
                }
            }
        }
    }

    post {
        success {
            echo "Deploy success: http://localhost:5009 (API) / http://localhost:3000 (Web)"
        }
        failure {
            echo "Pipeline failed. Check Jenkins logs."
        }
    }
}
