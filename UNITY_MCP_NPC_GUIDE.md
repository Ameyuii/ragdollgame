# Unity MCP Functions - NPC Management Guide

Hướng dẫn sử dụng Unity MCP functions để kiểm tra và xử lý NPCs hiệu quả.

## 🚀 Quick Start

### 1. Scripts đã được tạo:
- **NPCManagerEditor.cs** - Editor window cho quản lý NPCs
- **NPCMCPTester.cs** - Runtime testing và validation  
- **NPCValidator.cs** - Validation system cho NPCs
- **NPCControllerTests.cs** - Unit tests cho NPCController

### 2. Unity MCP Functions đã test:

#### ✅ execute_menu_item
```
Tạo GameObjects, Planes, menu items
Ví dụ: Tạo Empty GameObject, Primitive Objects
```

#### ✅ send_console_log  
```
Gửi log messages tới Unity Console
Ví dụ: Debug thông tin NPCs, status updates
```

#### ✅ add_asset_to_scene
```
Thêm prefabs từ project vào scene
Ví dụ: Thêm npc.prefab, npc1.prefab vào scene
```

#### ✅ update_component
```
Cập nhật properties của components
Ví dụ: Thay đổi team, health, attack damage của NPCs
```

#### ✅ run_tests
```
Chạy Unity Test Runner
Ví dụ: Test NPCController functionality
```

## 🎯 Cách sử dụng

### Trong Unity Editor:

1. **Compile scripts** - Đợi Unity compile xong
2. **Tools Menu**:
   - `Tools/NPC Manager` - Mở NPC management window
   - `Tools/NPC MCP/Quick Log NPCs` - Log thông tin NPCs nhanh
   - `Tools/NPC MCP/Create Test Environment` - Tạo test environment

3. **Context Menus** (Right-click trên component):
   - NPCValidator: `Validate All NPCs`, `Setup Combat Test`
   - NPCMCPTester: `Log All NPCs`, `Setup Test Combat`

### Runtime Testing:

1. **Thêm NPCValidator** vào scene
2. **Enable auto logging** để theo dõi NPCs
3. **Sử dụng context menus** để test nhanh

## 🔧 Test Scenarios

### Combat Test:
```
1. Tạo 2+ NPCs với teams khác nhau
2. Đặt chúng trong detection range
3. Quan sát combat behavior
```

### Validation Test:
```
1. Run NPCValidator.ValidateAllNPCs()
2. Kiểm tra console logs
3. Fix các issues được báo cáo
```

### Performance Test:
```
1. Tạo nhiều NPCs (10+)  
2. Monitor performance metrics
3. Optimize based on results
```

## 📊 MCP Functions Benefits

| Function | Benefit | Use Case |
|----------|---------|----------|
| execute_menu_item | Tự động hóa UI actions | Tạo objects, setup scene |
| send_console_log | Debug và monitoring | Track NPC states, errors |
| add_asset_to_scene | Quickly populate scene | Add NPCs, props, effects |
| update_component | Dynamic property changes | Adjust NPC stats, teams |
| run_tests | Automated validation | Ensure code quality |

## 🎮 Best Practices

1. **Luôn validate NPCs** trước khi test combat
2. **Sử dụng different teams** để tạo conflicts
3. **Monitor console logs** để debug issues
4. **Run tests regularly** để ensure quality
5. **Use MCP functions** cho repetitive tasks

## 🐛 Troubleshooting

### Common Issues:
- **Scripts không compile**: Check syntax errors
- **Menu items không xuất hiện**: Restart Unity Editor  
- **NPCs không interact**: Check teams và detection range
- **Tests fail**: Validate NPC setup

### Debug Steps:
1. Check Unity Console for errors
2. Run NPCValidator.ValidateAllNPCs()
3. Use MCP send_console_log for custom debugging
4. Run Unity Test Runner để check functionality

---

**Lưu ý**: Đảm bảo Unity Editor đã compile xong trước khi sử dụng các Tools menus và context menus.
