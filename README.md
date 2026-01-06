# 💿 MyMEDIA - Plataforma de Gestão de Suportes Multimédia

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![Blazor](https://img.shields.io/badge/Blazor-Hybrid-512BD4?logo=blazor)
![SQL Server](https://img.shields.io/badge/SQL_Server-2022-CC2927?logo=microsoft-sql-server)

O **MyMEDIA** é um ecossistema distribuído concebido para a gestão e comercialização de suportes multimédia (CDs, Vinis, Blu-Rays) e acessórios. [cite_start]Este projeto atua como um *marketplace* onde a empresa proprietária funciona como intermediária entre fornecedores e clientes finais.

## 🏗️ Arquitetura do Sistema

A solução segue uma arquitetura moderna e escalável, cumprindo o requisito de **código único** para múltiplas plataformas:

**`MyMedia.Shared (RCL)`**: Biblioteca de classes Razor que centraliza páginas, componentes e lógica de negócio, partilhada entre Web e Mobile.
**`MyMedia.API`**: Backend RESTful que gere a persistência de dados e segurança via **JWT**.
**`MyMedia.StoreManager`**: Aplicação de Backoffice com acesso direto à base de dados para gestão administrativa e validação de produtos.
**`MyMedia.Web`**: Interface pública para navegadores desenvolvida em Blazor WASM.
**`MyMedia.Mobile`**: Aplicação nativa para Android e Windows utilizando **.NET MAUI / Blazor Hybrid**.

## 🚀 Como Executar

Para testar o ecossistema completo no Visual Studio 2022:

1.  **Base de Dados**: Execute o script SQL incluído ou utilize o comando `Update-Database` na Package Manager Console.
2.  **Configuração de Inicialização**:
    * Clique com o botão direito na Solução > **Set Startup Projects**.
    * Selecione **Multiple Startup Projects**.
    * Defina `Action: Start` para os projetos: `MyMedia.API`, `MyMedia.StoreManager` e `MyMedia.Web`.
3.  **Dev Tunnel**: Certifique-se de que o Dev Tunnel está ativo para permitir a comunicação do emulador Android com a API local.

## 🔑 Utilizadores de Teste

| Perfil | Email | Password | Acesso |
| :--- | :--- | :--- | :--- |
| **Administrador** | admin@mymedia.pt | Admin123! | Backoffice |
| **Funcionário** | staff@mymedia.pt | Staff123! | Backoffice |
| **Fornecedor** | fornecedor@mymedia.pt | Forn123! | Frontend (Público) |
| **Cliente** | cliente@mymedia.pt | Cliente123! | Frontend (Público) |

## 🛠️ Principais Regras de Negócio

**Formação de Preço**: O preço final é a soma do preço base (fornecedor) com a margem definida pelo administrador.
**Estado Pendente**: Novos produtos inseridos por fornecedores requerem aprovação administrativa para ficarem ativos.
**Checkout**: A navegação é anónima, mas a finalização da compra exige autenticação de Cliente.

