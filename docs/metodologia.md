# Metodología de Trabajo y Gestión del Proyecto

## Marco de Trabajo: Scrum

El equipo trabajó bajo el marco de trabajo **Scrum**, organizando el desarrollo en **Sprints** de dos semanas. Cada Sprint tuvo un objetivo claro, un conjunto de Issues priorizados y una entrega funcional al final.

### Roles del Equipo

| Rol | Integrante | Responsabilidad |
| :--- | :--- | :--- |
| **Product Owner** | Profesor / Cliente | Definir y priorizar el Backlog. |
| **Scrum Master** | Persona 3 | Facilitar las ceremonias y auditar el tablero. |
| **Equipo de Desarrollo** | Persona 1 y Persona 2 | Implementar las funcionalidades. |

### Ceremonias Realizadas

- **Sprint Planning:** Al inicio de cada Sprint para definir el alcance y asignar Issues.
- **Daily Scrum:** Reuniones cortas diarias para sincronizar avances y bloqueos.
- **Sprint Review:** Al final de cada Sprint para demostrar los avances al Product Owner.
- **Sprint Retrospective:** Para identificar mejoras en el proceso.

---

## Gestión del Tablero (GitHub Projects)

El tablero se organizó con las siguientes columnas, que representan el estado de cada tarea:

| Columna | Significado |
| :--- | :--- |
| **Backlog** | Issues priorizados pero no asignados al Sprint actual. |
| **Epic** | Agrupación de Issues por funcionalidad grande (Épicas 1 a 4). |
| **In Progress** | Issues que se están desarrollando actualmente. |
| **Sprint 4** | Issues asignados al Sprint actual (Fase final). |
| **Done** | Issues completados y fusionados a `main`. |
| **Ready** | Issues listos para ser tomados por un desarrollador. |

Cada Issue fue vinculado a su rama y a su Pull Request, permitiendo trazabilidad completa del trabajo.

---

## Flujo de Trabajo en Git (Git Flow)

El equipo adoptó un flujo basado en ramas para mantener la estabilidad de `main`.

### Ramas

- **`main`:** Rama principal y estable. Solo recibe cambios a través de Pull Requests aprobados.
- **`feature/#[numero]-descripcion`:** Ramas individuales para cada Issue.

### Proceso para Contribuir

1. **Asignar Issue:** Cada miembro se asigna un Issue en GitHub Projects.
2. **Crear rama:** `git checkout -b feature/#[numero]-descripcion`
3. **Desarrollar y hacer commits** con mensajes claros.
4. **Subir la rama:** `git push origin feature/#[numero]-descripcion`
5. **Abrir Pull Request (PR):**
   - Título: `[Issue #N] Descripción breve`
   - Descripción: Explicar qué se hizo y cómo probarlo.
   - Vincular el Issue: `Closes #N`
6. **Revisión:** Al menos un compañero debe revisar y aprobar el PR.
7. **Fusionar (merge):** Una vez aprobado, se fusiona a `main` y el Issue se cierra automáticamente.

### Política de Commits

Los mensajes de commit siguen la convención de **Conventional Commits**:

- `feat:` para nuevas funcionalidades.
- `fix:` para correcciones de errores.
- `docs:` para documentación.
- `refactor:` para mejoras de código sin cambiar funcionalidad.
- `test:` para pruebas unitarias.

---

## Evidencia de Trazabilidad

- **Issues vinculados a PRs:** Cada Pull Request incluye la frase `Closes #N` para cerrar automáticamente el Issue al fusionarse.
- **Commits vinculados:** Los mensajes de commit hacen referencia al número del Issue (ej: `feat: implementar login (#69)`).
- **Ramas descriptivas:** El nombre de cada rama incluye el número del Issue y una descripción corta.
