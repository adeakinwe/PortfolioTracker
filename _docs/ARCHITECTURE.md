                         ┌──────────────┐
                         │  Controller  │
                         └──────┬───────┘
                                │
                   ┌────────────┴────────────┐
                   │                         │
                 CRUD             Business operation
                   │                         │
                   ▼                         ▼
          Repository Interface          Service
                   │                         │
                   ▼                  ┌──────┴──────┐
              Repository              │             │
                   │              Repository     Other
                   ▼              Interfaces     Services
               DbContext               │
                   │                   ▼
                   ▼               Repository
                 MySQL                 │
                                      ▼
                                  DbContext
                                      │
                                      ▼
                                    MySQL