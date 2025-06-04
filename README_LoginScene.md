# Login Scene – Quick Setup

1. Create **Login.unity** and add it to *Build Settings* (index 0).
2. UI hierarchy inside a Canvas:
   * InputField **Email** → `emailInput`
   * Button **Send Link** → `sendLinkButton`
   * InputField **Token** (paste token) → `tokenInput`
   * Button **Verify** → `verifyButton`
   * Text **Status** → `statusLabel`
3. Create empty GO **AuthController**, add **AuthManager** component, drag references.
4. Ensure **MainMenu.unity** scene exists and is also in build list.
5. Play‑test:  
   • Enter email → Send Link → watch server log for token URL.  
   • Paste token → Verify → loads MainMenu on success.

