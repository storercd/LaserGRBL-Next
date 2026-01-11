# LaserGRBL .NET 8 Migration - TODO List

## Critical (Must Fix Before .NET 9)

### 1. BinaryFormatter Serialization Migration
**Priority: HIGH** - BinaryFormatter is removed in .NET 9+

**Affected Files:**
- `LaserGRBL/Tools/Project.cs` - Project file serialization
- `LaserGRBL/Settings.cs` - Application settings persistence
- `LaserGRBL/Tools/Serializer.cs` - General serialization utilities

**Current Status:**
- Temporarily enabled with `EnableUnsafeBinaryFormatterSerialization=true`
- Security risk - BinaryFormatter has known vulnerabilities

**Recommended Solution:**
- Migrate to JSON serialization using `System.Text.Json` or `Newtonsoft.Json`
- Create migration path for existing .bin files:
  1. Detect old format on load
  2. Load with BinaryFormatter (if still available)
  3. Save in new JSON format
  4. Delete old .bin file

**Files to Migrate:**
- `%AppData%\LaserGRBL\LaserGRBL.Settings.bin`
- User project files (*.lprj)

**Effort:** 2-3 days

---

### 2. AsyncResult Compatibility Layer
**Priority: MEDIUM** - Currently throws NotSupportedException on .NET 8+

**Affected Files:**
- `LaserGRBL/ComWrapper/RJCP/SerialPortStream.cs` (lines 943, 1520)

**Current Status:**
- Serial port async operations fall back to throwing exceptions on .NET 8+
- May impact async serial communication

**Recommended Solution:**
- Implement proper Task-based async pattern for .NET 8+
- Use `Task.FromResult()` and `TaskCompletionSource` instead of AsyncResult
- Ensure backward compatibility with .NET Framework builds (if needed)

**Effort:** 1 day

---

## Important (Breaking Changes to Address)

### 3. Thread.Abort() Deprecation
**Priority: MEDIUM** - Throws PlatformNotSupportedException on .NET Core/.NET 5+

**Affected Files:**
- `LaserGRBL/Tools/ThreadClass.cs` (line 154)
- `LaserGRBL/UserControls/GrblPanel.cs` (line 163)
- `LaserGRBL/RasterConverter/ImageProcessor.cs` (line 869)

**Current Status:**
- Code compiles with warnings but will throw at runtime

**Recommended Solution:**
- Replace Thread.Abort() with cooperative cancellation using `CancellationToken`
- Refactor threads to check cancellation token and exit gracefully
- Use `Task` with cancellation instead of Thread where possible

**Effort:** 2-3 days

---

### 4. WebClient to HttpClient Migration
**Priority: LOW-MEDIUM** - WebClient is obsolete but still functional

**Affected Files:**
- `LaserGRBL/AutoUpdate/GitHub.cs` (4 instances)
- `LaserGRBL/UsageStats.cs`
- `LaserGRBL/Telegram.cs`
- `LaserGRBL/Ntfy.cs`
- `LaserGRBL/SvgLibrary/Basic Shapes/SvgImage.cs`
- `LaserGRBL/SvgLibrary/SvgElementIdManager.cs`

**Current Status:**
- SYSLIB0014 warnings throughout codebase
- WebClient will continue to work but is no longer recommended

**Recommended Solution:**
- Replace `WebClient` with `HttpClient` (use singleton instance)
- Use async/await pattern for all HTTP operations
- Add proper timeout and retry logic

**Effort:** 1-2 days

---

## Optional Improvements

### 5. Cryptography API Updates

**Affected APIs:**
- `SHA1Managed` → Use `SHA1.Create()`
- `SHA1CryptoServiceProvider` → Use `SHA1.Create()`
- `RNGCryptoServiceProvider` → Use `RandomNumberGenerator.Create()`
- `SymmetricAlgorithm.Create()` → Use specific algorithm (e.g., `Aes.Create()`)

**Files:**
- `LaserGRBL/ComWrapper/WebSocket/WebSocket.cs`
- `LaserGRBL/Tools/Serializer.cs`

**Effort:** 1 day

---

### 6. Code Access Security (CAS) Cleanup

**Affected Files:**
- `LaserGRBL/ComWrapper/WebSocket/Net/CookieException.cs`
- `LaserGRBL/ComWrapper/WebSocket/Net/WebHeaderCollection.cs`

**Current Status:**
- SYSLIB0003 warnings - CAS is not supported in .NET Core/.NET 5+
- Attributes are ignored at runtime

**Recommended Solution:**
- Remove all `[SecurityPermission]` attributes
- Remove CAS-related serialization code if not needed

**Effort:** 0.5 day

---

### 7. Assembly.CodeBase Deprecation

**Affected Files:**
- `LaserGRBL/Tools/OSHelper.cs` (line 22)

**Current Status:**
- SYSLIB0012 warning

**Recommended Solution:**
- Replace `Assembly.CodeBase` with `Assembly.Location`

**Effort:** 0.5 hour

---

### 8. X509Certificate2.PrivateKey Deprecation

**Affected Files:**
- `LaserGRBL/ComWrapper/WebSocket/Net/EndPointListener.cs` (line 206)

**Current Status:**
- SYSLIB0028 warning

**Recommended Solution:**
- Use `GetRSAPrivateKey()` or `CopyWithPrivateKey()` method

**Effort:** 0.5 hour

---

## Testing Recommendations

1. **Full Regression Test**
   - Test serial port communication (async operations)
   - Test project save/load after JSON migration
   - Test settings persistence
   - Verify all threading operations work correctly

2. **Migration Testing**
   - Test loading old .bin settings files
   - Test loading old project files
   - Verify data integrity after migration

3. **Performance Testing**
   - Benchmark JSON vs BinaryFormatter performance
   - Test threading changes don't impact responsiveness
   - Verify HTTP operations work correctly

---

## Build Configuration Notes

### Current Temporary Workarounds:
```xml
<!-- LaserGRBL.csproj -->
<EnableUnsafeBinaryFormatterSerialization>true</EnableUnsafeBinaryFormatterSerialization>
```

### Excluded Files:
- `SvgLibrary/Web/SvgHandler.cs` - ASP.NET web handler (not needed for desktop app)

### .NET 8 Conditional Compilation:
- Serial port AsyncResult code uses `#if NET8_0_OR_GREATER`
- Dynamic assembly creation uses `AssemblyBuilder.DefineDynamicAssembly()` on .NET 8+

---

## Estimated Total Effort

- **Critical Items:** 3-6 days
- **Important Items:** 3-5 days  
- **Optional Items:** 2-3 days
- **Total:** 8-14 days of focused development work

---

## Migration Strategy

**Phase 1 (Before Production):**
1. BinaryFormatter → JSON migration
2. Thread.Abort() → CancellationToken refactor

**Phase 2 (After Initial .NET 8 Release):**
3. AsyncResult proper implementation
4. WebClient → HttpClient migration

**Phase 3 (Cleanup):**
5. Cryptography API updates
6. Remove CAS attributes
7. Minor API deprecations

---
