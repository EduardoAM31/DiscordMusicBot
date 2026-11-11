# Sonora 🎵

Bot de música para Discord desenvolvido em **C# e .NET 10**, utilizando Discord.Net e Lavalink4NET para conexão com canais de voz, pesquisa e reprodução de músicas.

## Funcionalidades

Atualmente, o Sonora oferece os seguintes comandos:

| Comando | Descrição |
|---|---|
| `/ping` | Verifica se o bot está respondendo. |
| `/join` | Conecta o bot ao canal de voz do usuário. |
| `/leave` | Desconecta o bot do canal de voz. |
| `/play` | Pesquisa uma música pelo nome ou link do YouTube e inicia sua reprodução ou adiciona a faixa à fila. |
| `/pause` | Pausa a música atual. |
| `/resume` | Retoma a reprodução. |
| `/stop` | Interrompe a reprodução e limpa a fila. |
| `/skip` | Pula a música atual e avança para a próxima faixa, se houver. |

O bot não muda automaticamente de canal quando já está conectado a outro. Para mudar de canal, utilize `/leave` e depois `/join` ou `/play` no novo canal.

## Tecnologias utilizadas

- **C#** — linguagem de programação.
- **.NET 10** — plataforma de desenvolvimento.
- **Discord.Net 3.20.2** — biblioteca para comunicação com a API do Discord.
- **Lavalink 4.2.2** — servidor responsável pelo processamento e reprodução de áudio.
- **Lavalink4NET 4.2.2** — integração entre a aplicação e o Lavalink.
- **.NET User Secrets** — armazenamento local de credenciais durante o desenvolvimento.

## Arquitetura do projeto

A solução está organizada em três projetos principais:

```text
DiscordMusicBot/
├── src/
│   ├── DiscordMusicBot.Bot/
│   │   ├── Commands/
│   │   │   ├── General/
│   │   │   │   └── PingCommand.cs
│   │   │   └── Music/
│   │   │       ├── JoinCommand.cs
│   │   │       ├── LeaveCommand.cs
│   │   │       ├── PlayCommand.cs
│   │   │       ├── PauseCommand.cs
│   │   │       ├── ResumeCommand.cs
│   │   │       ├── StopCommand.cs
│   │   │       └── SkipCommand.cs
│   │   ├── Program.cs
│   │   └── DiscordMusicBot.Bot.csproj
│   │
│   ├── DiscordMusicBot.Core/
│   │   └── DiscordMusicBot.Core.csproj
│   │
│   └── DiscordMusicBot.Infrastructure/
│       ├── Discord/
│       │   └── DiscordBotClient.cs
│       └── DiscordMusicBot.Infrastructure.csproj
│
├── tests/
│   └── DiscordMusicBot.Tests/
│       ├── Core/
│       │   └── BasicTests.cs
│       └── DiscordMusicBot.Tests.csproj
│
├── DiscordMusicBot.slnx
├── .gitattributes
├── .gitignore
├── LICENSE.txt
└── README.md
```

**Responsabilidades dos projetos:**

- `Bot`: inicialização da aplicação, registro e execução dos comandos e integração com os serviços.
- `Core`: projeto destinado às entidades, interfaces e regras centrais da aplicação.
- `Infrastructure`: implementação de integrações externas, atualmente incluindo a conexão com o Discord.
- `Tests`: projeto destinado aos testes automatizados.

## Requisitos

Para executar o Sonora, são necessários:

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Java 21](https://adoptium.net/)
- [Lavalink 4.2.2](https://github.com/lavalink-devs/Lavalink)
- Uma aplicação e um bot criados no [Discord Developer Portal](https://discord.com/developers/applications).

## Configuração

### 1. Configurar o bot no Discord

Crie uma aplicação no [Discord Developer Portal](https://discord.com/developers/applications) e adicione um bot a ela.

Para convidar o bot ao servidor, gere um link de instalação utilizando os seguintes escopos OAuth2:

- `bot`
- `applications.commands`

Conceda ao bot as permissões necessárias para:

- Visualizar canais.
- Conectar-se a canais de voz.
- Falar em canais de voz.

A aplicação também utiliza os Gateway Intents `Guilds` e `GuildVoiceStates`, configurados diretamente no código.

### 2. Configurar o Lavalink

O Sonora utiliza o [Lavalink](https://github.com/lavalink-devs/Lavalink) como servidor independente para processamento e reprodução de áudio.

Instale e configure o Lavalink 4.2.2. O endereço utilizado atualmente pelo Sonora é:

```text
http://127.0.0.1:2333
```

A senha configurada no servidor Lavalink deve ser a mesma armazenada em `Lavalink:Password` nos User Secrets da aplicação.

O Lavalink deve estar em execução antes de iniciar o Sonora.

#### Plugin do YouTube

Para pesquisar e reproduzir músicas do YouTube, é necessário instalar o [YouTube Source Plugin](https://github.com/lavalink-devs/youtube-source), compatível com Lavalink 4.

Uma forma de instalar o plugin é declarar sua dependência no arquivo `application.yml`:

```yaml
server:
  port: 2333
  address: 127.0.0.1

lavalink:
  plugins:
    - dependency: "dev.lavalink.youtube:youtube-plugin:VERSAO_COMPATIVEL"
      snapshot: false

  server:
    password: "SUA_SENHA"
    sources:
      youtube: false

plugins:
  youtube:
    enabled: true
    allowSearch: true
    allowDirectVideoIds: true
    allowDirectPlaylistIds: true
```

Substitua `VERSAO_COMPATIVEL` por uma versão estável disponível na página de releases do plugin e `SUA_SENHA` por uma senha definida localmente.

A configuração acima é um exemplo inicial. As opções de clientes e reprodução podem precisar de ajustes conforme o comportamento do YouTube e a documentação do plugin.

**Importante:** o `application.yml` pode conter senhas e outras informações sensíveis. Mantenha-o fora do repositório e não compartilhe essas credenciais.

Referências:
- [Documentação do Lavalink](https://lavalink.dev/)
- [Instalação e configuração do YouTube Source Plugin](https://github.com/lavalink-devs/youtube-source)

### 3. Configurar as credenciais

O Sonora utiliza o **.NET User Secrets** para armazenar credenciais durante o desenvolvimento. Esses valores ficam fora da pasta do projeto e não são enviados ao controle de versão.

O projeto já possui um `UserSecretsId` configurado no arquivo `.csproj`. Portanto, não é necessário executar `dotnet user-secrets init` novamente.

Na raiz do repositório, execute os comandos abaixo, substituindo os valores de exemplo pelas suas próprias credenciais:

```bash
dotnet user-secrets set "Discord:Token" "SEU_TOKEN" --project ./src/DiscordMusicBot.Bot/DiscordMusicBot.Bot.csproj

dotnet user-secrets set "Discord:GuildId" "ID_DO_SERVIDOR" --project ./src/DiscordMusicBot.Bot/DiscordMusicBot.Bot.csproj

dotnet user-secrets set "Lavalink:Password" "SENHA_DO_LAVALINK" --project ./src/DiscordMusicBot.Bot/DiscordMusicBot.Bot.csproj
```

As chaves utilizadas correspondem às configurações lidas pela aplicação:

- `Discord:Token`: token do bot criado no Discord Developer Portal.
- `Discord:GuildId`: identificador do servidor Discord onde os comandos serão registrados.
- `Lavalink:Password`: senha configurada no servidor Lavalink.

**Atenção:** o User Secrets é destinado ao desenvolvimento local e não criptografa os valores armazenados. Não compartilhe o arquivo de segredos nem publique suas credenciais. Cada ambiente de desenvolvimento deve configurar seus próprios valores.

## Executando o projeto

Na raiz da solução, restaure as dependências:

```bash
dotnet restore ./DiscordMusicBot.slnx
```

Compile a solução:

```bash
dotnet build ./DiscordMusicBot.slnx
```

Com o Lavalink em execução, inicie o bot:

```bash
dotnet run --project ./src/DiscordMusicBot.Bot/DiscordMusicBot.Bot.csproj
```

Após a inicialização, o Sonora deverá conectar-se ao Discord e registrar os comandos no servidor configurado.

**Observação:** o endereço `127.0.0.1` pressupõe que o Sonora e o Lavalink estejam acessíveis no mesmo ambiente de rede. Caso sejam executados em máquinas ou contêineres separados, ajuste o endereço de conexão no `Program.cs`.

## Testes

Para executar os testes automatizados:

```bash
dotnet test ./DiscordMusicBot.slnx
```

## Segurança

- Não inclua tokens do Discord, senhas do Lavalink ou outros segredos no código-fonte.
- Não versione arquivos `secrets.json`, `.env` ou configurações locais que contenham credenciais.
- Utilize User Secrets durante o desenvolvimento.
- Mantenha as credenciais do servidor Lavalink protegidas.

## Licença

Este projeto está licenciado sob a [Licença MIT](LICENSE.txt).

---

Desenvolvido em C# com .NET 10.