# Arpscanner
ArpScanner é uma ferramenta avançada de varredura de rede escrita em C# com foco em segurança ofensiva e análise de redes corporativas. Ele combina técnicas de Port Scanning (TCP) com detecção de dispositivos via ARP Scanning, oferecendo uma visão detalhada da infraestrutura de rede em ambientes IPv4.


---

## Funcionalidades

- ARP scanning para detecção de hosts ativos em redes locais.
- Varredura de portas TCP com detecção de banners.
- Suporte a intervalos de IPs em notação CIDR (ex: `192.168.0.0/24`).
- Controle de concorrência via `SemaphoreSlim`.
- Exportação dos resultados em JSON formatado.

---

## Compilação

Requisitos:
- [.NET 6.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

Para compilar:

```bash
dotnet build

Para executar:

dotnet run --project ./src/portScanner-arpScanner.csproj -- \
    --target 192.168.1.0/24 \
    --start-port 20 \
    --end-port 1024 \
    --threads 100 \
    --output results.json

Parâmetros
Parâmetro	Descrição
--target	IP único ou bloco CIDR (ex: 192.168.0.0/24)
--start-port	Porta inicial a ser escaneada
--end-port	Porta final a ser escaneada
--threads	Número máximo de operações simultâneas
--output	Caminho para salvar o relatório em formato JSON
Estrutura do Projeto

portScanner-arpScanner/
├── src/
│   ├── Program.cs
│   ├── Scanner.cs
│   ├── ServiceDetector.cs
│   ├── TargetParser.cs
│   ├── ReportWriter.cs
│   └── Models/
│       └── ScanResult.cs
├── README.md
├── portScanner-arpScanner.csproj

Saída

O resultado do escaneamento será salvo em formato JSON no caminho especificado com o seguinte modelo:

[
  {
    "IP": "192.168.1.10",
    "Port": 22,
    "Status": "Open",
    "Banner": "OpenSSH 8.2"
  },
  {
    "IP": "192.168.1.15",
    "Port": 80,
    "Status": "Open",
    "Banner": "Apache/2.4.29"
  }
]

Licença

Este projeto está licenciado sob a Licença MIT. Consulte o arquivo LICENSE para mais detalhes.


---

Se quiser, posso adaptar esse `README` para Markdown com suporte ao GitHub Pages, gerar um `Makef
