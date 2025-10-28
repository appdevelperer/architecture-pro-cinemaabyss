1.  Запуск локально ранеров через act:  
    act workflow_dispatch --secret GITHUB_TOKEN=$(gh auth token)
2.  Мой Personal Access Token (PAT) https://github.com/settings/tokens : 
    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
3.  Описание сервисов доступно при развертывании в docker:
    - монолит: http://localhost:8080/api/movies
    - movie-service: http://localhost:8081/api/movies
    - proxy-service: http://localhost:8000/api/movies
4.  При развертывании в k8s:
    - event-service: http://cinemaabyss.example.com/api/events/health