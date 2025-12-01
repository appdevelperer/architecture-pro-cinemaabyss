1.  Запуск локально ранеров через act (НЕ ТРЕБУЕТСЯ В ДАННОМ ПРОЕКТЕ):  
    act workflow_dispatch --secret GITHUB_TOKEN=$(gh auth token)
2.  Мой Personal Access Token (PAT) https://github.com/settings/tokens : 
    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
3.  Описание сервисов доступно (при развертывании в docker):
    - монолит: http://localhost:8080/api/movies
    - movie-service: http://localhost:8081/api/movies - возвращает список фильмов из сервиса
    - proxy-service: http://localhost:8000/api/proxy
    - event-service: http://localhost:8082/openapi/index.html
4.  Запуск k8s
    - Project_template, швг 2
5.  При развертывании в k8s:
    - event-service: http://cinemaabyss.example.com/api/events/health
    - movie-service: https://cinemaabyss.example.com/api/movies
6. Запуск тестов для контейнеров docker
    - imiroedov@miroedov:~/Development/sprint_2/architecture-pro-cinemaabyss/tests/postman$ ./run-tests.sh
7.  Запуск тестов для k8s
    - перейти в tests/postman 
    - выполнить npm run test:kubernetes
8.  как посмотреть логи пода в кубере
    - kubectl logs <pod-name> [-n <namespace>]
    - kubectl logs monolith-8476598495-45kkm -n cinemaabyss
    - kubectl describe pod monolith-8476598495-45kkm -n cinemaabyss --Если еще не стартовал контейнер изза ошибки

9.  helm install cinemaabyss ./src/kubernetes/helm \
  --namespace cinemaabyss \
  --create-namespace
10. Нужно установить ingress controller
    kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.8.2/deploy/static/provider/cloud/deploy.yaml
    kubectl wait --namespace ingress-nginx --for=condition=ready pod --selector=app.kubernetes.io/component=controller --timeout=120s