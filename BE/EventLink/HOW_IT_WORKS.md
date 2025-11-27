# 📖 Cách Code Hoạt Động và Format Response

## 🔄 Workflow Tổng Quan

```
User Request
    ↓
POST /api/chatai/match-partnerships
    ↓
Extract User ID từ message
    ↓
Get User Role từ database
    ↓
┌─────────────────┬─────────────────┐
│   Organizer     │     Sponsor      │
└────────┬────────┴────────┬─────────┘
         │                 │
    Get Events      Get Brand Profile
         │                 │
    Get Sponsor     Get Organizer
    Partnerships    Partnerships
         │                 │
    Calculate       Calculate
    Match Scores    Match Scores
         │                 │
    Top 3-5         Top 3-5
    Matches         Matches
         │                 │
         └────────┬────────┘
                  │
         Format Response
         (Claude Haiku)
                  │
         Return JSON
```

## 📝 Chi Tiết Từng Bước

### Bước 1: Extract User ID

**Input:**
```json
{
  "message": "Tôi là user có id 12345678-1234-1234-1234-123456789012"
}
```

**Code:**
```csharp
ExtractUserIdFromMessage(request.Message)
// Pattern: "có id [GUID]" hoặc "id [GUID]"
// Returns: Guid? (nullable)
```

**Output:** `12345678-1234-1234-1234-123456789012`

---

### Bước 2: Get User Role

**Code:**
```csharp
var userRoleResult = await _partnershipMatchingService.GetUserRoleAsync(userId.Value);
// Query: SELECT * FROM Users WHERE Id = '[USER_ID]'
```

**Output:**
- `user`: User object (FullName, Email, Role, etc.)
- `role`: "Organizer" hoặc "Sponsor"

---

### Bước 3A: Nếu Role = "Organizer"

#### 3A.1: Get Events
```csharp
var events = await _eventService.GetEventsByOrganizerIdAsync(userId.Value);
// Query: SELECT * FROM Events WHERE OrganizerId = '[USER_ID]'
```

**Returns:** `IEnumerable<EventResponse>`
- Title, Description, Location, EventDate
- TotalBudget, ExpectedAttendees
- EventType, Category, Tags

#### 3A.2: Find Sponsor Matches
```csharp
var matches = await _partnershipMatchingService.FindSponsorMatchesAsync(userId.Value);
```

**Process:**
1. Get all Sponsor partnerships (Status = "Pending" hoặc "Ongoing")
2. Join với BrandProfiles để lấy thông tin brand
3. **Calculate match score** cho mỗi event với mỗi sponsor:
   - Industry Match (30 điểm)
   - Keywords Match (30 điểm)
   - Budget Compatible (20 điểm)
   - Location Match (20 điểm)
   - Semantic Match (10 điểm)
4. Filter: chỉ lấy matches có score >= 40
5. Sort: theo score descending
6. Take: top 5 matches

**Returns:** `List<SponsorMatchResult>`
```csharp
{
    Partnership: Partnership object,
    BrandProfile: BrandProfile object,
    Event: Event object,
    Score: 85,
    Reasons: ["Ngành Technology khớp...", "Keywords trùng khớp..."],
    Stars: "⭐⭐⭐⭐⭐"
}
```

#### 3A.3: Format Response
```csharp
response = await FormatOrganizerResponseAsync(user, events.ToList(), matches);
```

**Process:**
1. Tạo system prompt cho Claude
2. Tạo user prompt với:
   - User info (FullName, Email)
   - Events list
   - Matches với scores và reasons
3. Gọi Claude Haiku API
4. Claude format response theo Vietnamese format
5. Return formatted text

---

### Bước 3B: Nếu Role = "Sponsor"

#### 3B.1: Get Brand Profile
```csharp
var brandProfile = await _brandProfileService.GetByUserIdAsync(userId.Value);
// Query: SELECT * FROM BrandProfiles WHERE UserId = '[USER_ID]'
```

**Returns:** `BrandProfileResponse`
- BrandName, Industry, CompanySize
- Tags, OurMission, Location

#### 3B.2: Find Organizer Matches
```csharp
var matches = await _partnershipMatchingService.FindOrganizerMatchesAsync(userId.Value);
```

**Process:**
1. Get all Organizer partnerships (Status = "Pending" hoặc "Ongoing")
2. Join với Events để lấy thông tin event
3. **Calculate match score** cho brand profile với mỗi organizer partnership:
   - Industry Match (30 điểm)
   - Keywords Match (30 điểm)
   - Budget Compatible (20 điểm)
   - Location Match (20 điểm)
   - Semantic Match (10 điểm)
4. Filter: chỉ lấy matches có score >= 40
5. Sort: theo score descending
6. Take: top 5 matches

**Returns:** `List<OrganizerMatchResult>`

#### 3B.3: Format Response
```csharp
response = await FormatSponsorResponseAsync(user, brandProfile, matches);
```

**Process:** Tương tự như Organizer nhưng format cho Sponsor

---

## 🎯 Matching Algorithm

### Scoring Logic

```csharp
// High Priority (30 points each)
- Industry Match: So sánh Industry/Category fields
- Keywords Match: So sánh Tags/Keywords fields

// Medium Priority (20 points each)
- Budget Compatible: Ratio >= 0.7
- Location Match: Same city/region

// Low Priority (10 points)
- Semantic Match: Common words >= 3

Total: 100 points
```

### Example Calculation

**Event:**
- Category: "Technology"
- Tags: "AI, Machine Learning, Tech"
- Location: "Ho Chi Minh City"
- TotalBudget: 100,000,000

**Sponsor:**
- Industry: "Technology"
- Tags: "AI, Tech, Innovation"
- Location: "Ho Chi Minh City"
- ProposedBudget: 80,000,000

**Score:**
- Industry Match: ✅ 30 điểm (Technology = Technology)
- Keywords Match: ✅ 30 điểm (AI, Tech trùng)
- Budget Compatible: ✅ 20 điểm (80M/100M = 0.8 >= 0.7)
- Location Match: ✅ 20 điểm (HCMC = HCMC)
- Semantic Match: ✅ 10 điểm (common words)
- **Total: 90 điểm** ⭐⭐⭐⭐⭐

---

## 📤 Response Format

### API Response Structure

```json
{
  "success": true,
  "role": "Organizer",
  "response": "📊 Phân tích thông tin của bạn:\n- Vai trò: Organizer\n..."
}
```

### Response Content (Vietnamese Text)

#### Cho Organizer:

```
📊 **Phân tích thông tin của bạn:**
- Vai trò: Organizer
- Họ tên: [FullName]
- Email: [Email]
- Số sự kiện: [X] sự kiện

**Danh sách sự kiện của bạn:**
1. [Event Title] - [EventType]
   - Quy mô: [ExpectedAttendees] người
   - Địa điểm: [Location]
   - Thời gian: [EventDate]
   - Ngân sách: [TotalBudget]

---

🎯 **Tìm thấy [X] đối tác Sponsor phù hợp với sự kiện của bạn:**

**1. [BrandName] - [ServiceDescription]** ⭐⭐⭐⭐⭐ (85 điểm)

📋 **Thông tin Partnership:**
- Loại: Sponsor
- Ngành: [Industry]
- Từ khóa: [Tags]
- Địa điểm: [Location]
- Ngân sách: [ProposedBudget]
- Trạng thái: [Status]

💼 **Thông tin Brand:**
- Tên thương hiệu: [BrandName]
- Quy mô công ty: [CompanySize]
- Sứ mệnh: [OurMission summary]

✨ **Lý do phù hợp:**
• Ngành [Industry] khớp với loại sự kiện [EventType]
• Keywords "[tags]" trùng khớp
• Ngân sách tương thích
• Cùng khu vực [Location]

📝 **Yêu cầu từ Sponsor:** [ServiceDescription]
🎁 **Lợi ích họ cung cấp:** [ServiceDescription]

---

[Repeat for top 3-5 matches]
```

#### Cho Sponsor:

```
📊 **Phân tích thông tin của bạn:**
- Vai trò: Sponsor
- Họ tên: [FullName]
- Email: [Email]
- Thương hiệu: [BrandName]
- Ngành: [Industry]
- Quy mô: [CompanySize]
- Từ khóa: [Tags]
- Địa điểm: [Location]
- Sứ mệnh: [OurMission summary]

---

🎯 **Tìm thấy [X] cơ hội tài trợ sự kiện phù hợp:**

**1. [Event Title]** ⭐⭐⭐⭐⭐ (90 điểm)

📋 **Thông tin Partnership:**
- Loại: Organizer (đang tìm sponsor)
- Mô tả: [ServiceDescription]
- Địa điểm: [Location]
- Ngân sách: [ProposedBudget]
- Trạng thái: [Status]

✨ **Lý do phù hợp:**
• Sự kiện về [Category] khớp với ngành [Industry] của bạn
• Keywords "[event tags]" match với tags "[brand tags]"
• Cùng khu vực [Location]
• Ngân sách tương thích

📝 **Yêu cầu từ Organizer:** [ServiceDescription]
🎁 **Lợi ích bạn nhận được:** [ServiceDescription]

---

[Repeat for top 3-5 matches]
```

---

## 🔧 Technical Details

### Services Used

1. **PartnershipMatchingService**
   - Extract User ID
   - Get User Role
   - Find matches
   - Calculate scores

2. **ClaudeService**
   - Format response bằng Claude Haiku
   - Model: `claude-3-5-haiku-20241022`
   - Max tokens: 4096

3. **EventService / BrandProfileService**
   - Get events/brand profiles từ database

### Database Queries

**Get User Role:**
```sql
SELECT * FROM Users WHERE Id = '[USER_ID]'
```

**Get Events (Organizer):**
```sql
SELECT * FROM Events WHERE OrganizerId = '[USER_ID]'
```

**Get Sponsor Partnerships:**
```sql
SELECT p.*, bp.*
FROM Partnerships p
INNER JOIN Users u ON p.PartnerId = u.Id
INNER JOIN BrandProfiles bp ON u.Id = bp.UserId
WHERE p.PartnerType = 'Sponsor' 
  AND (p.Status = 'Pending' OR p.Status = 'Ongoing')
```

**Get Organizer Partnerships:**
```sql
SELECT p.*, e.*
FROM Partnerships p
LEFT JOIN Events e ON p.EventId = e.Id
WHERE p.PartnerType = 'Organizer'
  AND (p.Status = 'Pending' OR p.Status = 'Ongoing')
```

---

## 📊 Example Flow

### Request:
```http
POST /api/chatai/match-partnerships
Content-Type: application/json
Authorization: Bearer [JWT_TOKEN]

{
  "message": "Tôi là user có id 12345678-1234-1234-1234-123456789012"
}
```

### Processing:
1. Extract User ID: `12345678-1234-1234-1234-123456789012`
2. Get Role: `"Organizer"`
3. Get Events: 2 events
4. Get Sponsor Partnerships: 10 partnerships
5. Calculate scores: 5 matches >= 40 điểm
6. Format với Claude: Vietnamese text

### Response:
```json
{
  "success": true,
  "role": "Organizer",
  "response": "📊 Phân tích thông tin của bạn:\n- Vai trò: Organizer\n- Họ tên: John Doe\n- Email: john@example.com\n- Số sự kiện: 2 sự kiện\n\n**Danh sách sự kiện của bạn:**\n1. Tech Summit 2024 - Conference\n   - Quy mô: 500 người\n   - Địa điểm: Ho Chi Minh City\n   - Thời gian: 15/03/2024\n   - Ngân sách: 100,000,000\n\n---\n\n🎯 **Tìm thấy 3 đối tác Sponsor phù hợp với sự kiện của bạn:**\n\n**1. TechCorp - Premium Sponsorship Package** ⭐⭐⭐⭐⭐ (90 điểm)\n\n📋 **Thông tin Partnership:**\n- Loại: Sponsor\n- Ngành: Technology\n- Từ khóa: AI, Tech, Innovation\n- Địa điểm: Ho Chi Minh City\n- Ngân sách: 80,000,000\n- Trạng thái: Pending\n\n💼 **Thông tin Brand:**\n- Tên thương hiệu: TechCorp\n- Quy mô công ty: Large\n- Sứ mệnh: Leading technology solutions provider...\n\n✨ **Lý do phù hợp:**\n• Ngành Technology khớp với loại sự kiện Conference\n• Keywords \"AI, Tech\" trùng khớp với focus sự kiện\n• Ngân sách tương thích (100,000,000 vs 80,000,000)\n• Cùng khu vực Ho Chi Minh City\n\n📝 **Yêu cầu từ Sponsor:** Premium branding opportunities\n🎁 **Lợi ích họ cung cấp:** Premium branding opportunities\n\n---\n\n[2 more matches...]"
}
```

---

## ⚙️ Configuration

### Environment Variables

```env
CLAUDE_API_KEY=your_claude_api_key_here
DB_CONNECTION=Server=...;Database=...;...
```

### Service Registration

```csharp
// Program.cs
builder.Services.AddSingleton<ClaudeService>();
builder.Services.AddScoped<IPartnershipMatchingService, PartnershipMatchingService>();
```

---

## 🎯 Key Features

1. **Reliable Matching**: Pure C# logic, không phụ thuộc AI behavior
2. **Smart Scoring**: Multi-criteria scoring (Industry, Keywords, Budget, Location, Semantic)
3. **Vietnamese Response**: Claude format response tiếng Việt đẹp
4. **Top Matches**: Chỉ show matches >= 40 điểm, top 3-5
5. **Detailed Reasoning**: Giải thích cụ thể tại sao match

---

## 🔍 Debug Tips

1. **Check User ID extraction**: Log `ExtractUserIdFromMessage` output
2. **Check matches**: Log `matches.Count` và scores
3. **Check Claude response**: Log raw Claude API response
4. **Check database**: Verify data trong SQL Server

