# sjvs — Simple Java Version Switcher

sjvs es una herramienta CLI ligera para gestionar y cambiar versiones de JDK en Windows mediante JAVA_HOME, usando un directorio configurable de instalaciones.

## Características

- Gestión de múltiples JDKs desde un directorio central
- Cambio de JAVA_HOME por versión
- Soporte para `latest` (última versión)
- Configuración persistente del directorio de JDKs
- Funciona sin modificar PATH
- Herramienta portable (solo un .exe + config)
## Estructura recomendada

```
sjvs.exe
sjvs.config (se crea automáticamente)
jdks/
   jdk-17
   jdk-21.0.2
   jdk-23.0.2
```
## Instalación y compilación

### 1. Requisitos

.NET SDK 6+ (recomendado .NET 8)

Verificar:

```bash
dotnet --version
```
### 2. Crear proyecto

```bash
dotnet new console -n sjvs
cd sjvs
```

### 3. Reemplazar código

Sustituye el contenido de `Program.cs` por el código de la herramienta.

### 4. Compilar en Release

```bash
dotnet build -c Release
```

### 5. Ejecutable final

El .exe se encuentra en:

```
bin\Release\net10.0\sjvs.exe
```

Puedes copiarlo a cualquier carpeta junto con tu directorio de JDKs.

## Comandos disponibles

### Configurar directorio de JDKs

```bash
sjvs dir <path>
```

Define dónde están instalados los JDKs.

**Ejemplo:**

```bash
sjvs dir D:\Java\jdks
```
### Listar JDKs disponibles

```bash
sjvs list
```

Muestra todas las carpetas dentro del directorio configurado.

**Ejemplo:**

```
JDKs disponibles:
 - jdk-17
 - jdk-21.0.2
 - jdk-23.0.2
```
### Activar una versión

```bash
sjvs use <version>
```

Cambia JAVA_HOME al JDK indicado.

**Ejemplo:**

```bash
sjvs use jdk-21.0.2
```
### Usar la última versión

```bash
sjvs use latest
```

Selecciona automáticamente el JDK con la versión más alta.

### Ver JAVA_HOME actual

```bash
sjvs current
```

Muestra el valor actual de JAVA_HOME del usuario.

### Comportamiento de matching

El comando `use` soporta:

**Coincidencia exacta:**

```bash
sjvs use jdk-23.0.2
```

**Coincidencia parcial:**

```bash
sjvs use 23
sjvs use jdk-23
```

**Selección automática:**

```bash
sjvs use latest
```
### Configuración interna

El directorio de JDKs se guarda en `sjvs.config`

**Contenido:**

```
D:\Java\jdks
```
## Ejemplo completo de uso

```bash
sjvs dir D:\Java\jdks
sjvs list
sjvs use jdk-23.0.2
sjvs current
sjvs use latest
```
## Notas

- Solo modifica JAVA_HOME del usuario (no requiere admin)
- No modifica PATH
- Requiere reiniciar terminales para reflejar cambios en nuevos procesos
- Diseñado para ser portable

## Futuras mejoras posibles

- `sjvs reset` (borrar configuración)
- `sjvs where` (mostrar config actual)
- Validación de `java.exe`
- Soporte semver avanzado
- Autocompletado en PowerShell
