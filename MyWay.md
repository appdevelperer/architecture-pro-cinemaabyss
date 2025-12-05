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
11. Подробный лог пода
    - kubectl describe pod istio-ingressgateway-667447b56f-r282x -n istio-system

12. Удаление Namespace
    - kubectl delete namespace istio-system

13. Мой вариант установки istio
    # Добавить репозиторий
    helm repo add istio https://istio-release.storage.googleapis.com/charts
    helm repo update

    # Проверить доступные версии
    helm search repo istio --versions | grep "1.20"

    helm install istio-base istio/base \
    --version 1.20.0 \
    -n istio-system \
    --set defaultRevision=default \
    --wait


    helm install istiod istio/istiod \
    --version 1.20.0 \
    -n istio-system \
    --set global.hub=docker.io/istio \
    --set global.tag=1.20.0 \
    --set pilot.autoscaleEnabled=false \
    --set pilot.resources.requests.memory=256Mi \
    --set pilot.resources.requests.cpu=100m \
    --wait



    helm install istio-ingressgateway istio/gateway \
    --version 1.20.0 \
    -n istio-system \
    --set global.hub=docker.io/istio \
    --set global.tag=1.20.0 \
    --set "podAnnotations.sidecar\.istio\.io/inject=\"false\"" \
    --set "service.type=LoadBalancer" \
    --set "service.ports[0].name=http" \
    --set "service.ports[0].port=80" \
    --set "service.ports[0].targetPort=8080" \
    --set "service.ports[1].name=https" \
    --set "service.ports[1].port=443" \
    --set "service.ports[1].targetPort=8443"

    # На control-plane ноде скачать образ
    docker pull docker.io/istio/proxyv2:1.20.0

    # Проверить что скачался
    docker images | grep proxyv2