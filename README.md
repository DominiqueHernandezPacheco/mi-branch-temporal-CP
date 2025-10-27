# PLATILLA PARA UN SISTEMA CONECTADO A LLAVE CAMPECHE

<p align="center">
    <img alt="Recurso 15@2x(2)" src="REFERENCIA_AL_ASSET_DEL_PROYECTO">
</p>

![.Net](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white)
![JWT](https://img.shields.io/badge/JWT-black?style=for-the-badge&logo=JSON%20web%20tokens)
[![Despliegue Pipeline](https://github.com/Sds-Dti/SolicitudesSDA_BackEnd/actions/workflows/Pipeline.yml/badge.svg)](https://github.com/Sds-Dti/SolicitudesSDA_BackEnd/actions/workflows/Pipeline.yml)

## Requisitos para Ejecutar el Proyecto

> **Importante**: Para usuarios de **VS 2022**, asegúrate de tener la versión más reciente para evitar errores en el proyecto.

### SDK de .NET Core 7
Instala el SDK de .NET Core 7 utilizando el siguiente comando:

```bash
winget install Microsoft.DotNet.SDK.7
```

### Dotnet CLI 

```bash
dotnet tool install --global dotnet-ef
```
## EF Core

### Scaffold  a la base de datos

Para aplicar la ingeniería reversa de la base de datos y poder mapear todas las entidades, podemos hacer uso del  `scaffold` de `ef core`. Esto funciona bien cuando usamos el enfoque de `database first`,  o bien cuando queremos realizar algún cambio grande de la base de datos, que es complicado de realizar en código. 

```bash
dotnet ef dbcontext scaffold "Data Source=172.19.2.130;Initial Catalog=SEUP;User Id=sa;password=123456789;Encrypt=False;" Microsoft.EntityFrameworkCore.SqlServer --output-dir Domain\Entities --context-dir Infraestructure\Persistance --context SEUPDbContext --project Application.Core\Application.Core.csproj
```
_________________
### Crear las migraciones

Para crear una nueva migración, el comando sugerido es el siguiente:

**.NetCore CLI**
```bash
dotnet ef migrations add "MigracionInicial" --project Application.Core --startup-project Presentation --output-dir Infraestructure\Persistance\Migrations
```

**Visual Studio**


```bash
Add-Migration "MigracionInicial" --project Application.Core --startup-project Application.Core --output-dir Infraestructure\Persistance\Migrations
```

Donde:

- `--project Application.Core`: el proyecto que contiene el `dbcontext`.

- `--startup-project Application.Core`: proyecto inicial de ejecución.

- `--output-dir Infraestructure\Persistance\Migrations`:  el directorio de destino que almacenará las migraciones.

_________________

### Aplicar las migraciones a la base de datos

En caso de que se quiera probar una versión en local, aplicar un nuevo parche o se migre la base de datos a otro servidor, el comando sugerido sería el siguiente:

**.NetCore CLI**
```bash
dotnet ef database update --project Application.Core --startup-project Presentation
```

**Visual Studio**

```bash
Update-Database --project Application.Core --startup-project Presentation
```

Donde:

- `--project Application.Core`: se conoce como proyecto de destino porque es donde los comandos añaden o eliminan archivos. Por defecto, el proyecto en el directorio actual es el proyecto de destino. Puede especificar un proyecto diferente como proyecto de destino utilizando la opción `--project`.

- `--startup-project Presentation` : el proyecto de inicio es el que las herramientas construyen y ejecutan. Las herramientas tienen que ejecutar código de aplicación en tiempo de diseño para obtener información sobre el proyecto, como la cadena de conexión a la base de datos y la configuración del modelo. Por defecto, el proyecto en el directorio actual es el proyecto de inicio. Puede especificar un proyecto diferente como proyecto de inicio utilizando la opción `--startup-project`.

> Ver más en [uso de la herramientas](https://learn.microsoft.com/en-us/ef/core/cli/dotnet#using-the-tools) de Net Core.
 
Estos parámetros no serían necesarios si todo lo relacionado a EF Core lo estuviésemos trabajando en el mismo proyecto, pero no es el caso, estamos usando dos proyectos para separar las capas lógicas. Tal vez te sea útil saber que estamos creando el `dbcontext` en tiempo de diseño desde los [servicios de aplicación](https://learn.microsoft.com/en-us/ef/core/cli/dotnet#using-the-tools).  Si has revisado el proyecto, entonces te habrás dado cuenta de que en el `src/Presentation/Presentation.csproj` no contiene ninguna referencia a EF Core o sus pares:

```xml
<ItemGroup>  
 <PackageReference Include="Serilog.AspNetCore" Version="6.1.0" />  
 <PackageReference Include="Swashbuckle.AspNetCore.SwaggerGen" Version="6.4.0" />  
 <PackageReference Include="Swashbuckle.AspNetCore.SwaggerUI" Version="6.4.0" />  
</ItemGroup>

```

lo cual tiene sentido ya que la capa de presentación no debería tener esas referencias que serían innecesarias (siendo puristas a los principios de la arquitectura clean). Sin embargo, para superar este conflicto fue necesario fue modificar el archivo `src/Application.Core/Application.Core.csproj` añadiendo el valor `none` dentro de la etiqueta `PrivateAssets`.

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="6.0.11">  
 <PrivateAssets>none</PrivateAssets>  
 <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>  
</PackageReference>
```

Ya que de lo contrario te encontrarías con el siguiente mensaje de error si intentas ejecutar alguno de los comandos anteriores:

`Your startup project 'Presentation' doesn't reference Microsoft.EntityFrameworkCore.Design. This package is required for the Entity Framework Core Tools to work. Ensure your startup project is correct, install the package, and try again.
`

> Ver [Referencing Microsoft.EntityFrameworkCore.Design](https://learn.microsoft.com/en-us/ef/core/cli/services#referencing-microsoftentityframeworkcoredesign) para más detalles.

> Ver este [hilo](https://stackoverflow.com/questions/52536588/your-startup-project-doesnt-reference-microsoft-entityframeworkcore-design/62663918#62663918) que abarca el problema del proyecto de inicio.

_________________

### Remover las migraciones

Para remover alguna migración que haya sido agregada por error o algún otro motivo 😉. Usa el siguiente comando:

```bash
dotnet ef migrations remove --project Application.Core --startup-project Presentation
```

> **Note**: la sección `startup-project src\Presentation` ya que de lo contrario obtendríamos el siguiente error:  `No database provider has been configured for this DbContext. A provider can be configured by overriding the 'DbContext.OnConfiguring' method or by using 'AddDbContext' on the application service provider. If 'AddDbContext' is used, then also ensure that your DbContext type accepts a DbContextOptions<TContext> object in its constructor and passes it to the base constructor for DbContext.`
