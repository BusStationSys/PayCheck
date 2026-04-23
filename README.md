# PayCheck

## 📋 Sobre o Projeto / About the Project

PayCheck é um sistema de gerenciamento de folha de pagamento e recursos humanos desenvolvido em .NET 7.0. O sistema permite que colaboradores e empregadores gerenciem demonstrativos de pagamento, espelhos de ponto, e informações de matrícula de forma centralizada e segura.

**PayCheck** is a payroll and human resources management system built with .NET 7.0. The system allows employees and employers to manage payment statements, time records, and enrollment information in a centralized and secure manner.

## 🏗️ Arquitetura / Architecture

O projeto segue uma arquitetura em camadas:

- **PayCheck.Web**: Aplicação web MVC para interface do usuário
- **PayCheck.Api**: API RESTful para integração e serviços
- **ARVTech.DataAccess**: Camadas de acesso a dados, serviços e domínio

## 🚀 Tecnologias / Technologies

- .NET 7.0
- ASP.NET Core MVC
- ASP.NET Core Web API
- SQL Server
- Entity Framework (via ARVTech.DataAccess)

## 📖 Funcionalidades / Features

- **Gestão de Usuários**: Autenticação e gerenciamento de perfis
- **Colaboradores**: Cadastro e gestão de pessoas físicas (colaboradores)
- **Empregadores**: Cadastro e gestão de pessoas jurídicas (empresas)
- **Demonstrativos de Pagamento**: Visualização e gerenciamento de contracheques
- **Espelho de Ponto**: Controle de frequência e horas trabalhadas
- **Notificações**: Sistema de alertas para usuários
- **Relatórios**: Gráficos de evolução e composição salarial

## ❓ FAQ - Perguntas Frequentes

### O que seria esses "agentes" no código?

**Resposta**: Não existem "agentes de IA" (inteligência artificial) neste sistema. As referências a "agente" encontradas no código comentado (por exemplo, em `UsuarioController.cs`) são resquícios de uma implementação antiga que foi substituída pelo conceito de "Usuário".

Historicamente, o sistema pode ter utilizado o termo "agente" para se referir a um tipo de entidade de negócio (possivelmente um representante ou agente da empresa), mas essa funcionalidade foi removida e substituída pelo módulo de gerenciamento de usuários atual.

**Em resumo**: 
- ❌ NÃO há agentes de inteligência artificial neste sistema
- ❌ NÃO há funcionalidades de IA ou machine learning
- ✅ "Agente" era uma entidade de negócio antiga, agora obsoleta
- ✅ O sistema usa "Usuário" como conceito principal de identificação

### What are these "agents" in the code?

**Answer**: There are NO "AI agents" (artificial intelligence) in this system. References to "agente" found in commented code (e.g., in `UsuarioController.cs`) are remnants of an old implementation that was replaced with the "User" concept.

Historically, the system may have used the term "agente" to refer to a type of business entity (possibly a representative or company agent), but this functionality was removed and replaced by the current user management module.

**Summary**:
- ❌ There are NO artificial intelligence agents in this system
- ❌ There are NO AI or machine learning features
- ✅ "Agente" was an old business entity, now obsolete
- ✅ The system uses "User" as the main identification concept

## 🔧 Como Executar / How to Run

### Pré-requisitos / Prerequisites

- .NET 7.0 SDK
- SQL Server
- Visual Studio 2022 ou Visual Studio Code

### Configuração / Setup

1. Clone o repositório
2. Configure a string de conexão no `appsettings.json`
3. Execute as migrations do banco de dados
4. Execute o projeto

```bash
dotnet restore
dotnet build
dotnet run --project src/PayCheck.Web
```

## 📝 Licença / License

Este projeto está sob licença. Veja o arquivo LICENSE para mais detalhes.

## 🤝 Contribuindo / Contributing

Contribuições são bem-vindas! Por favor, siga as práticas padrão de desenvolvimento .NET e envie pull requests para revisão.
