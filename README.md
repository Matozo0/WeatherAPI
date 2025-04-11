<h1 align="center">
  <br>
  WeatherAPI
  <br>
</h1>

<h4 align="center">
  API REST em ASP.NET Core para receber e armazenar leituras de dispositivos IoT.
</h4>

<p align="center">
  <a href="#sobre">Sobre</a> -
  <a href="#recursos-atual">Recursos (atual)</a> -
  <a href="#executar-localmente">Executar localmente</a> -
  <a href="#todo">TODO</a>
</p>

## Sobre
A WeatherAPI é uma API que vai guardar e mostrar dados de clima (temperatura, umidade, pressão, luz etc.) que chegam de sensores IoT — como placas ESP, dispositivos Tuya ou qualquer outro que envie dados por MQTT ou HTTP.

O que ainda vamos adicionar:
- MongoDB para guardar as medições dos sensores de forma rápida e organizada;
- MQTT para receber as leituras em tempo real (ex.: weather/{deviceId}/telemetry);
- Dashboards (Grafana, por exemplo) para ver gráficos e históricos;
- Alertas quando algum valor passar dos limites definidos;
- Um arquivo docker‑compose que sobe tudo de uma vez (API, Mongo, MQTT, etc.).

## Recursos (atual)

| Funcionalidade | Status |
|----------------|--------|
| **JWT Bearer Auth** – políticas `Admin` e `User` configuradas no pipeline |✔️ 
| **Swagger UI** – documentação automática em `/swagger` (modo dev) | ✔️
| **Persistência MySQL** – `DbContext` com migrações e *auto‑migrate* na inicialização | ✔️
| **Seed de usuário Admin** – cria usuário root na primeira execução | ✔️|

> **Observação:** o back‑end ainda **não** recebe métricas de sensores; o foco atual foi a fundação (auth, banco, Swagger).

## Executar localmente

```bash
git clone https://github.com/Matozo0/WeatherAPI.git
cd WeatherAPI

# variáveis de ambiente (exemplo)
export KEY_TOKEN="uma‑chave‑secreta‑bem‑grande"
export ConnectionStrings__DefaultConnection="server=localhost;user=root;password=123;database=weatherdb"

dotnet restore
dotnet ef database update   # cria o schema MySQL
dotnet run                  # https://localhost:5001
````

Depois de rodar, acesse **/swagger** para ver os endpoints e testar logins.

## TODO

* [ ] **Armazenamento MongoDB** para séries temporais de métricas ambientais
* [ ] **Broker MQTT** (Mosquitto) + tópico `weather/+/telemetry`
* [ ] **WebHooks Tuya & ESP RainMaker** – parsers dedicados para cada payload
* [ ] **Cache Redis** para últimas leituras por dispositivo
* [ ] **Jobs Hangfire** para cálculo de médias/históricos
* [ ] **Docker Compose** com API + Mongo + Mosquitto + Redis
* [ ] **Painel Grafana** via datasource Mongo / Prometheus
* [ ] **Tests** (xUnit + WebApplicationFactory)