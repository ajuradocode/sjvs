# sjvs — Simple Java Version Switcher

sjvs es una herramienta CLI ligera para gestionar y cambiar versiones de JDK en Windows mediante JAVA_HOME, usando un directorio configurable de instalaciones.

## Características

- Gestión de múltiples JDKs desde un directorio central
- Cambio de JAVA_HOME por versión
- Solo modifica JAVA_HOME del usuario (no requiere admin)
- No modifica PATH
- Soporte para `latest` (última versión)
- Configuración persistente del directorio de JDKs
- Funciona sin modificar PATH. Se da por hecho que ya se encuentra añadida la entrada %JAVA_HOME%\bin
- Herramienta portable (solo un .exe + config)

## Instalación

Descargamos la última release y la ubicamos en una ubicación de nuestra conveniencia (p.e. C:\Tools\sjvs). Posteriormente, añadimos la ruta a la variable de entorno PATH

## Uso

```
NAME:
   sjvs - Simple Java Version Switcher for Windows

USAGE:
   sjvs.exe [global options] command [command options] [arguments...]

COMMANDS:
     dir             Configure the JDK directory
     list            List available JDK installations
     use             Switch to use the specified version.
     current         Show the current JAVA_HOME

GLOBAL OPTIONS:
   --help, -h        Show help
   --version, -v     Print the current sjvs version
```

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

### Ejemplo de uso completo

```bash
sjvs dir D:\Java\jdks
sjvs list
sjvs use jdk-23.0.2
sjvs current
sjvs use latest
```

## 🔧 Futuras mejoras posibles

- `sjvs reset` (borrar configuración)
- `sjvs where` (mostrar config actual)
- Validación de `java.exe`
- Soporte semver avanzado
- Autocompletado en PowerShell
