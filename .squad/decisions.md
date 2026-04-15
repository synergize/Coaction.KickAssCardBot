# Squad Decisions

## Active Decisions

### Layout Strategy Refactor Approval

The refactor of `MTGCardOutputManager.cs` to use the strategy pattern for layout handling is approved. The public API is preserved, and no behavioral regressions were found. The new structure improves extensibility and maintainability for future layout types.

— Keaton

### MTG Output Refactor Details

Moved inline layout handler logic from MTGCardOutputManager into a dedicated NormalLayoutHandler under Embed Output\LayoutHandlers.

Registered handlers in MTGCardOutputManager constructor: TransformLayoutHandler and NormalLayoutHandler.

No behavioral changes to AddEmojis; only refactoring to remove inline handler classes and centralize layout handlers.

— Fenster

## Governance

- All meaningful changes require team consensus
- Document architectural decisions here
- Keep history focused on work, decisions focused on direction
