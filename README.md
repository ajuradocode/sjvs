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

## Uso

```
NAME:
   sjvs - Simple Java Version Switcher

USAGE:
   sjvs.exe [global options] command [command options] [arguments...]

VERSION:
   0.1.0

COMMANDS:
     dir             Configure the JDK directory
     list            List available JDK installations
     use             Switch to use the specified version.
     current         Show the current JAVA_HOME

GLOBAL OPTIONS:
   --help, -h     show help
   --version, -v  print the version
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

## Notas

- Solo modifica JAVA_HOME del usuario (no requiere admin)
- No modifica PATH
- Diseñado para ser portable

## 🔧 Futuras mejoras posibles

- `sjvs reset` (borrar configuración)
- `sjvs where` (mostrar config actual)
- Validación de `java.exe`
- Soporte semver avanzado
- Autocompletado en PowerShell
