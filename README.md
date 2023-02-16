# ACE2_1S23_G0

# _Estación Meteorológica IOT_ 
Se solicita la creacion de un dispositivo meteorológico inteligente para pronosticar y tomar las debidas precauciones acorde a las condiciones meteorológicas

Como ya es de costumbre hoy día, los datos generados y almacenados por cualquier dispositivo deben poder ser monitoreados, visibles y de fácil comprensión para cualquier tipo de usuario, desde el más experto hasta el más inexperto, por ello se decidió también integrar una interfaz que permita interpretar las lecturas de una forma gráfica y animada.

### Objetivos
- Diseñar un dispositivo destinado a medir y registrar regularmente, diversas variables meteorológicas.
- Implementar una aplicación en Processing que permita visualizar mediciones y observaciones puntuales de los diferentes parámetros meteorológicos.
- Aprender a desarrollar una manera correcta de visualización de datos mediante la implementación del framework de IoT.


# API

Para poder manejar los datos que se obtengan del dispositivo se decidió trabajar con una API (Interfaz de programación de aplicaciones) por la cual se pueda realizar la comunicación del dispositivo hacia una base de datos mediante comunicación serial y poder mantener la información obtenida.

La API se desarrollo en el lenguaje **_C#_** utilizando la tecnología del framework **_.NET_** utilizando el template de Minimal API, para la base de datos se utilizo SQL Server en dicha base de datos se almacenaran todos los datos leídos por el dispositivo, dichos datos se recolectaran mediante sensores, para la comunicación entre la base de datos y el framework se utilizo entity framework el cual es una _ORM_.

### Requisitos para el funcionamiento 
- [Descargar e instalar el SDK de **.NET**](https://dotnet.microsoft.com/es-es/download)
- Descargar e instalar [VSC](https://code.visualstudio.com/download) para la manipulación del programa
- [Postman](https://www.postman.com/downloads/) o cualquier programa para manipular API's
- [Descargar e instalar SQL Server 2019](https://www.visual-expert.com/ES/visual-expert-blog/posts-2020/guia-instalacion-sql-server-2019-visual-expert.html)
- [Descargar e instalar SQL Server Management Studio (SSMS)](https://learn.microsoft.com/es-es/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver16) 

### Preparando el entorno
Una vez tengamos todo instalado necesitaremos realizar una descarga de las dependencias del proyecto estas dependencias podemos instalarlas utilizando el siguiente comando en la terminal, tenemos que estar en la ubicación del proyecto.
``` 
dotnet restore
```
Si esto no funciona también podemos utilizar el archivo llamado "**dependencias.txt**" en el cual encontraremos los comandos para poder instalar las dependencias de nuestro proyecto.
```
//para EF
dotnet add package Microsoft.EntityFrameworkCore --version 7.0.2

//para conexion de sql server
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 7.0.2

//para el puerto serial
dotnet add package System.IO.Ports --version 7.0.0

//newton soft para json
dotnet add package DocaLabs.Http.Client.with.NewtonSoft.Json.Serializer --version 3.0.0

//esto para la instalacion de dependencias
dotnet restore
```
# Iniciando el funcionamiento de la API
Para hacer que nuestra API comience a funcionar bastara con utilizar el comando
```
dotnet run
```
## Alojamiento del proyecto📌
-[GitHub](https://github.com)

## Sistema de control de versiones 📌
- [Git](https://git-scm.com)

## Desarrolladores 🧑‍💻
* **Tomás Alexnader Morales Saquic** - *Estudiante de Ing. Ciencias y Sistemas* - [*GitHub*](https://github.com/AlejoMora991014) - [*LinkedIn*](https://www.linkedin.com/in/tomas-morales-saquic-1431ba22b/)

* **Rony Ormandy Ortíz Alvarez** - *Estudiante de Ing. Ciencias y Sistemas* - [GitHub](https://github.com/OrmandyRony)
* **Jhonatan Josué Tzunún Yax** - *Estudiante de Ing. Ciencias y Sistemas*
* **Elder Fernando Andrade** - *Estudiante de Ing. Ciencias y Sistemas*

* **Anotarse ...**
