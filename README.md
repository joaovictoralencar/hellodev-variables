# HelloDev Variables

A ScriptableObject-backed variable system for designer-friendly data sharing with automatic change event support.

## Features

- **SO Variables**: Type-safe variables (Float, Int, Bool, String, or custom types) backed by ScriptableObjects
- **Change Events**: Automatic `UnityEvent<T>` raised when values change
- **Generic API**: Inherit from `Variable_SO<T>` for any type
- **Quick Creation**: Create variables inline with a single click in the Inspector
- **Auto-Reset**: Variables automatically reset to default values between play sessions

## Quick Start

### Creating Variables (Designer)

1. **Inline Creation** (Recommended):
   ```csharp
   public class PlayerHealth : MonoBehaviour
   {
       [SerializeField] private FloatVariable_SO health;
   }
   ```
   - Inspector shows a "Create" button when the field is null
   - Click → auto-creates `Assets/Variables/PlayerHealth_Float_Variable_SO.asset` and assigns it

2. **Manual Creation**:
   - Right-click in Project window → Create > HelloDev > Variables > [Type] Variable
   - Or use the menu: Assets > Create > HelloDev > Variables > [Type] Variable

### Using Variables (Programmer)

```csharp
public class GameManager : MonoBehaviour
{
    [SerializeField] private IntVariable_SO playerScore;

    void Start()
    {
        // Subscribe to changes
        playerScore.OnValueChanged.AddListener(OnScoreChanged);

        // Set value (triggers event if changed)
        playerScore.Value = 100;

        // Reset to default
        playerScore.ResetToDefault();
    }

    void OnScoreChanged(int newScore)
    {
        Debug.Log($"Score: {newScore}");
    }
}
```

## Creating Custom Variable Types

```csharp
using UnityEngine;
using UnityEngine.Events;
using HelloDev.Variables;

// Create a variable for any type
[CreateAssetMenu(menuName = "HelloDev/Variables/Vector3 Variable")]
public class Vector3Variable_SO : Variable_SO<Vector3>
{
    // Optional: override ValuesEqual for custom comparison logic
    protected override bool ValuesEqual(Vector3 a, Vector3 b)
    {
        return Vector3.Distance(a, b) < 0.001f;
    }
}
```

## API Reference

### VariableBase_SO
Base class for all variables.

```csharp
public abstract void ResetToDefault();
```

### Variable_SO<T>
Generic base class for typed variables.

```csharp
// Get/set value (property)
public T Value { get; set; }

// Set value if different (explicit method)
public void SetValue(T newValue)

// Fires UnityEvent<T> when value changes
public UnityEvent<T> OnValueChanged

// Reset to default value
public override void ResetToDefault()

// Override for custom comparison logic
protected virtual bool ValuesEqual(T a, T b)
```

### Built-in Types
- `FloatVariable_SO` - Uses `Mathf.Approximately()` for comparison
- `IntVariable_SO` - Basic equality comparison
- `BoolVariable_SO` - Basic equality comparison
- `StringVariable_SO` - Uses `string.Equals()` for comparison

## Inspector Features

- **Create Button**: Auto-generates SO when field is null (inline)
- **Reset Button**: Available in SO inspector via custom editor
  - Resets value to default in both Edit and Play modes
  - Marks asset as dirty (saves changes)

## Folder Structure

```
Runtime/
├── Scripts/
│   ├── VariableBase_SO.cs          ← Non-generic base with reset logic
│   ├── Variable_SO.cs              ← Generic base for all types
│   ├── FloatVariable_SO.cs
│   ├── IntVariable_SO.cs
│   ├── BoolVariable_SO.cs
│   └── StringVariable_SO.cs

Editor/
├── Scripts/
│   ├── VariablePropertyDrawer.cs   ← Inline "Create" button
│   └── VariableEditor.cs           ← Reset button in SO inspector
```

## Dependencies

- **com.hellodev.utils** (1.2.0+) - RuntimeScriptableObject base class

## License

MIT - See LICENSE file

---

**Next Steps:**
- Create variables for your project's specific needs
- Subscribe to `OnValueChanged` to react to value updates
- Use in combination with HelloDev Conditions for rule-based logic
