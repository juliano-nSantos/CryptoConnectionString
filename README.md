
# CriptoConnectionString

Uma bibliote .Net para criptografar connection strings em tempo de execução.
Lê o arquivo .env e caso não tenha cria um automaticamente. Caso também não tenha uma cliente_key e cliente_iv cria essas chaves poins elas são utilizadas na criptografia da connection string.
Lê e criptografa todos os nós da chave ConnectionStrings

Faz isso dinamicamente sem a necessidade de fazer a criptografia externamente e colocar manualmente no appsettings.json.

Descriptografa somente quando for utilizar e fazer a conexão com o banco de dados

## Importante

	Quando inicia a aplicação pela primeira vez é validado se existe arquivo .env, cliente_key e cliente_iv. Se não tiver é criado o arquivo e as chaves e inserido esses valores no arquivo .env.
As chaves cliente_key e cliente_iv são utilizadas na criptografia e Descriptografia da connectionStrings, então se houver alteração dessas chaves a Descriptografia não ira funcionar.

## Instalação

Disponivel em [NuGet](https://www.nuget.org/packages/CryptoConnectionString/)

Visual Studio:

```powershell
PM> Install-Package CryptoConnectionString
```

.Net Core CLI:

```bash
dotnet add package CryptoConnectionString
```

## Usando

Na Program.cs use o metodo AddCryptoConnectionString(). Por padrão essa função procurará automaticamente um arquivo .env no atual diretorio da aplicação.

```csharp
	builder.Services.AddCryptoConnectionString(builder.Configuration);
```

Ou você pode especificar o caminho diretamente do arquivo '.env'

```csharp
	builder.Services.AddCryptoConnectionString(builder.Configuration, "./path/.env");
```

# Descriptografando 

Você pode usar a interface IConnectionStringEncryptor que já está sendo registrada no metodo AddCryptoConnectionString() ou instanciar a classe  ConnectionStringEncryptor

```csharp
	private readonly IConnectionStringEncryptor _connectionStringEncryptor
	
	public Exemplo(IConnectionStringEncryptor connectionStringEncryptor)
	{
		_connectionStringEncryptor=connectionStringEncryptor;
	}
	
	var connString = _connectionStringEncryptor.Decrypt("Valor a ser descriptografado");	
```

Ou

```csharp
	var encryptor = new ConnectionStringEncryptor();
	
	var connString = encryptor.Decrypt("Valor a ser descriptografado");
```



## Licença

Esse projeto está licenciado sob a licença MIT.