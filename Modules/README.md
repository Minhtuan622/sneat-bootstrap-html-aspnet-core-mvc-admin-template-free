# Module-oriented structure

Refactor from layer-first to feature/module-first layout.

## Implemented modules
- `Modules/Orders/`
  - queue import flow (controller/model/repository/service/worker)
- `Modules/Reports/`
  - report send flow (controller/services/worker/report repositories+models)
- `Modules/Observability/`
  - health, audit, error log controllers + error log service/repository/model
- `Modules/Auth/`
  - auth controller, user repository/models, password hasher

## Benefits
- feature code is co-located
- easier maintenance and onboarding
- incremental migration toward microservice-friendly boundaries

## Next module candidates
- `Modules/Dashboard/`
- `Modules/LiveConfigs/`
- `Modules/SystemSettings/`
- `Modules/Facebook/`
