# ANALISIS DE LOGICA DE NEGOCIO – LCWPS

## Orquestacion y Notificacion en Tiempo Real con Verificacion Manual de Pago

**Autoridad:** Product Owner, Domain Expert, Chief Architect
**Clasificacion:** BUSINESS CRITICAL
**Fecha:** 2026-02-23
**Idioma:** Espanol (corporativo)
**Estado:** ✓ APROBADO

---

## 1. Definicion Actual del Sistema

LCWPS es un **sistema de orquestacion y notificacion en tiempo real** para la gestion de turnos clinicos. La verificacion de pago es **manual y obligatoria** antes de ingresar a la cola activa.

**No es:** un sistema de validacion financiera automatica ni un motor de decision automatizado.

---

## 2. Entidad Central: Turno (Appointment)

El Turno es la entidad principal del dominio. Todo el flujo clinico depende de su estado y eventos asociados.

### 2.1 Estados del Turno

- `Created`
- `PaymentPendingVerification`
- `PaymentVerified`
- `PaymentRejected`
- `Waiting`
- `Called`
- `InService`
- `Completed`
- `Cancelled`
- `NoShow`

### 2.2 Invariantes Criticas

- Ningun turno puede pasar a `Waiting` sin verificacion manual previa.
- Los cambios de estado generan eventos y auditoria inmutable.
- La prioridad es automatica y no editable.

---

## 3. Verificacion Manual de Pago

### 3.1 Principio Operativo

La verificacion de pago se realiza fuera del sistema (finanzas externas) y se confirma manualmente en LCWPS por roles autorizados.

### 3.2 Flujo Obligatorio

1. Turno creado → `Created`
2. Transicion automatica → `PaymentPendingVerification`
3. Usuario autorizado verifica pago externo
4. Accion en LCWPS:
   - **Aprobar** → `PaymentVerified` → `Waiting` (automatico)
   - **Rechazar** → `PaymentRejected` (terminal)
5. Auditoria inmutable y evento emitido

### 3.3 Roles Autorizados

- `FinancialVerifier`
- `Supervisor`
- `Admin`

---

## 4. Priorizacion y FIFO

### 4.1 Priorizacion Automatica

- Menor de 18 anos → **HIGH**
- Embarazada → **HIGH**
- Mayor o igual a 65 anos → **HIGH**
- Otros casos → **NORMAL**

### 4.2 FIFO Estricto por Prioridad

- Primero todos los **HIGH** por orden de creacion.
- Luego todos los **NORMAL** por orden de creacion.
- Prohibido reordenamiento manual.

---

## 5. Motor de Notificaciones en Tiempo Real

### 5.1 Eventos Notificables

- Creacion de turno
- Verificacion o rechazo de pago
- Entrada a `Waiting`
- Llamado `Called`
- Inicio `InService`
- Finalizacion `Completed`
- Cancelacion `Cancelled`
- `NoShow`

### 5.2 Reglas de Entrega

- Latencia objetivo < 1 segundo
- Fallos de notificacion no bloquean el flujo
- Reintentos configurables
- Idempotencia obligatoria

---

## 6. Gestion del Llamado

1. Turno en `Waiting`
2. Staff ejecuta accion **Call**
3. Estado cambia a `Called`
4. Notificacion inmediata
5. Si responde → `InService`
6. Si no responde tras intentos configurados → `NoShow`

---

## 7. Multi-Tenant

El aislamiento multi-tenant es total:

- Datos
- Usuarios
- Configuracion
- Notificaciones
- Auditoria

No existe visualizacion cruzada entre tenants.

---

## 8. Auditoria Inmutable

Eventos auditables obligatorios:

- Creacion de turno
- Verificacion manual de pago (aprobacion o rechazo)
- Cambios de estado
- Acciones de llamado
- Cambios de rol
- Intentos fallidos

Retencion minima: 5 anos.

---

## 9. Glosario

- **Turno (Appointment):** Entidad central del dominio.
- **Priority:** Clasificacion automatica (HIGH/NORMAL).
- **FIFO:** Orden estricto dentro de cada prioridad.
- **Manual Verification:** Confirmacion humana del pago.
- **Tenant:** Clinica u organizacion aislada.

---

**Estado del Documento:** ✓ APROBADO
**Revision:** Trimestral
**Proxima Revision:** 2026-05-23
