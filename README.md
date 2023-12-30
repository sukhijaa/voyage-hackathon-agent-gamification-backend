# Getting Started with Agent Gamification

This is the Backend repo for Agent Gamification Idea
For UI Repo, please refer https://github.com/sukhijaa/voyage-hackathon-agent-gamification-ui 

## Live Version
Checkout the Running POC on http://ec2-15-206-79-201.ap-south-1.compute.amazonaws.com:8100/

## Demo Video
Youtube https://youtu.be/m7RUv1xkanY

## Available Scripts

### Docker Build
docker build -t agentgamification:latest -f Dockerfile .

### Docker Run
docker run -d -e AWS_REGION=ap-south-1 -p 8080:8080 -p 5601:5601 agentgamification:latest

### Local Environment
Open the project in Visual Studio 2020
Run the Program.cs file
Application will come live on localhost:5056