# Modular Feature Structure

This project has been reorganized using a **feature/module-first** layout to avoid horizontal growth (`Controllers/`, `Services/`, `Repositories/` all mixed together).

## Module tree (phase 1)

```
Modules/
├── Auth/
│   ├── Controllers/
│   ├── Models/
│   ├── Repositories/
│   └── Services/
├── Dashboard/
│   ├── Controllers/
│   ├── Models/
│   ├── Repositories/
│   └── Services/
├── LiveConfigs/
│   ├── Controllers/
│   ├── Models/
│   ├── Repositories/
│   └── Services/
├── Reports/
│   ├── Controllers/
│   ├── Models/
│   ├── Repositories/
│   └── Services/
└── Shared/
    ├── Controllers/
    ├── Models/
    ├── Repositories/
    └── Services/
```

## Rules for next refactors

1. New business features should be added under `Modules/<FeatureName>/...`.
2. Keep each module self-contained (controller + model + repository + service).
3. Reusable cross-module logic should go to `Modules/Shared`.
4. Perform migration incrementally to keep routing and views stable.

## Notes

- Current MVC view paths are unchanged (`Views/...`) to avoid breaking default view discovery.
- Namespaces were kept compatible to minimize risk while moving files.
