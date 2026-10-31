# Tienda Don Julio / TiendaBarrio

Sistema integral de gestión comercial y e-commerce para tiendas de barrio desarrollado en .NET 10 con Blazor Server en el frontend, ASP.NET Core Web API en el backend y PostgreSQL 16 como motor de base de datos relacional.

## Integrantes del Equipo
* **Persona 1:** Backend, Entity Framework Core & OpenAPI
* **Persona 2:** Frontend Blazor Server, UI/UX & Dockerization
* **Persona 3:** QA, Pruebas Unitarias & Documentación Arquitectónica

## Gestión del Proyecto
* **Tablero de Seguimiento (GitHub Projects):** [Ver Tablero de GitHub Projects](https://github.com/orgs/jpcc1217/projects)
* **Especificación OpenAPI:** `openapi.json` (ubicado en la raíz del repositorio)

---

## Instrucciones de Despliegue con Docker

El sistema está completamente contenedorizado y se puede desplegar ejecutando un solo comando desde un clon limpio en la raíz del repositorio:

```bash
docker compose up -d --build