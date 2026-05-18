-- SafiStore - Assign real Unsplash images to ALL products
-- Run directly on your Monster DB
-- Uses specific images.unsplash.com photo URLs (stable, NOT random)

-- ============================================================
-- COMPUTERS / LAPTOPS
-- ============================================================

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1517336714731-489689fd1ca8?w=800'
WHERE Title LIKE '%MacBook Pro 16%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1593642632823-8f785ba67e45?w=800'
WHERE Title LIKE '%Dell XPS%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1603302576837-37561b5f0a8a?w=800'
WHERE Title LIKE '%ASUS ROG%' OR Title LIKE '%Razer Blade%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1496181133206-80ce9b88a853?w=800'
WHERE Title LIKE '%Laptop%' OR Title LIKE '%Notebook%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1525547719571-a2d4ac8945e2?w=800'
WHERE (Title LIKE '%MacBook%' OR Title LIKE '%MacBook Pro%') AND ImageUrl IS NULL;

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1591799264318-7e6ef8ddb7fa?w=800'
WHERE Title LIKE '%ThinkPad%' OR Title LIKE '%Lenovo%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1588872657578-7efd1f1555ed?w=800'
WHERE Title LIKE '%HP%' OR Title LIKE '%Spectre%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1611186871348-b1f696febc21?w=800'
WHERE Title LIKE '%Surface%';

-- ============================================================
-- TABLETS / iPads
-- ============================================================

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1544244015-0df4b3ffc6b0?w=800'
WHERE Title LIKE '%iPad Air%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1540659918981-5402f79c6ee8?w=800'
WHERE Title LIKE '%iPad Pro%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1561154464-82e9adf32764?w=800'
WHERE Title LIKE '%iPad%' AND ImageUrl IS NULL;

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1567581935884-3349723552ca?w=800'
WHERE Title LIKE '%Galaxy Tab%' OR Title LIKE '%Samsung Tab%' OR Title LIKE '%Tablet%';

-- ============================================================
-- MONITORS / DISPLAYS
-- ============================================================

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1527443224154-c4a3942d3acf?w=800'
WHERE Title LIKE '%Monitor%' OR Title LIKE '%Display%' OR Title LIKE '%UltraWide%';

-- ============================================================
-- PHONES
-- ============================================================

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1695048133142-1a735a44f0b7?w=800'
WHERE Title LIKE '%iPhone 15 Pro Max%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1592286927505-1def25115558?w=800'
WHERE Title LIKE '%iPhone 15%' AND ImageUrl IS NULL;

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1511707171634-5f897ff02aa9?w=800'
WHERE Title LIKE '%iPhone%' AND ImageUrl IS NULL;

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1705491197419-1b3e98f82d73?w=800'
WHERE Title LIKE '%Samsung Galaxy S24 Ultra%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1610945415295-d9bbf067e59c?w=800'
WHERE Title LIKE '%Samsung Galaxy S24%' AND ImageUrl IS NULL;

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1598327105666-5b89351aff97?w=800'
WHERE (Title LIKE '%Samsung%' OR Title LIKE '%Galaxy%') AND Title NOT LIKE '%Tab%' AND Title NOT LIKE '%Watch%' AND ImageUrl IS NULL;

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1696426057727-4e0d6d5d0b8c?w=800'
WHERE Title LIKE '%Google Pixel%' OR Title LIKE '%Pixel 8%' OR Title LIKE '%Pixel 9%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1585060544812-183b067099c0?w=800'
WHERE Title LIKE '%OnePlus%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1589492477829-5e65395b66cc?w=800'
WHERE Title LIKE '%Xiaomi%' OR Title LIKE '%Redmi%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1609220136736-443140cffec6?w=800'
WHERE Title LIKE '%Huawei%' OR Title LIKE '%Honor%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1598965402089-897ce52e8355?w=800'
WHERE Title LIKE '%Nothing Phone%';

-- ============================================================
-- AUDIO - HEADPHONES
-- ============================================================

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1618366712010-f4ae9c647dcb?w=800'
WHERE Title LIKE '%Sony WH%' OR Title LIKE '%Sony%Headphone%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1606220945775-6a7831d5f4f3?w=800'
WHERE Title LIKE '%AirPods Pro%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1588423771073-b8903fbb85b5?w=800'
WHERE Title LIKE '%AirPods%' AND ImageUrl IS NULL;

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=800'
WHERE Title LIKE '%Bose%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1583394838336-acd977736f90?w=800'
WHERE Title LIKE '%Beats%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1558756520-22cfe5d382ca?w=800'
WHERE (Title LIKE '%Headphone%' OR Title LIKE '%Headset%') AND ImageUrl IS NULL;

-- ============================================================
-- AUDIO - EARBUDS
-- ============================================================

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1590658268037-6bf12f032f55?w=800'
WHERE Title LIKE '%Earbuds%' OR Title LIKE '%Earphone%';

-- ============================================================
-- AUDIO - SPEAKERS
-- ============================================================

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1608043152269-423dbba4e7e1?w=800'
WHERE Title LIKE '%JBL%' OR Title LIKE '%Flip%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1558089687-f282ffcbc126?w=800'
WHERE Title LIKE '%Sonos%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1589003077984-894e133dabab?w=800'
WHERE Title LIKE '%Speaker%' OR Title LIKE '%Soundbar%';

-- ============================================================
-- WEARABLES - WATCHES
-- ============================================================

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1694566382298-205f3e1f8e14?w=800'
WHERE Title LIKE '%Apple Watch Ultra%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1546868871-af0de0ae72d1?w=800'
WHERE Title LIKE '%Apple Watch%' AND ImageUrl IS NULL;

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1694999624021-5c0c4f3f0a9e?w=800'
WHERE Title LIKE '%Galaxy Watch 6 Classic%' OR Title LIKE '%Galaxy Watch 6%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1523275335684-37898b6baf30?w=800'
WHERE Title LIKE '%Garmin%' OR Title LIKE '%Fenix%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1579586337278-3befd40fd17a?w=800'
WHERE (Title LIKE '%Smartwatch%' OR Title LIKE '%Watch%') AND ImageUrl IS NULL;

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1557935728-e6d1eaabe558?w=800'
WHERE Title LIKE '%Fitness%Tracker%' OR Title LIKE '%Fitbit%';

-- ============================================================
-- GAMING
-- ============================================================

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1606813907291-d86efa9b94db?w=800'
WHERE Title LIKE '%PlayStation%' OR Title LIKE '%PS5%' OR Title LIKE '%PS4%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1621259182978-fbf93132d53d?w=800'
WHERE Title LIKE '%Xbox%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1578303512597-81e6cc155b3e?w=800'
WHERE Title LIKE '%Nintendo%' OR Title LIKE '%Switch%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1592840496694-26d035b52b48?w=800'
WHERE Title LIKE '%Controller%' OR Title LIKE '%Gamepad%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1493711662062-fa541adb3fc8?w=800'
WHERE Title LIKE '%Gaming%Console%' OR Title LIKE '%Console%';

-- ============================================================
-- ACCESSORIES
-- ============================================================

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1629429408209-1f912961dbd8?w=800'
WHERE Title LIKE '%Logitech MX%' OR Title LIKE '%MX Master%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1589782182703-2aaa69037b5b?w=800'
WHERE Title LIKE '%Keyboard%' OR Title LIKE '%Mechanical%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1527864550417-7fd91fc51a46?w=800'
WHERE Title LIKE '%Mouse%' AND Title NOT LIKE '%MX Master%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1613141411244-0e4ac259d217?w=800'
WHERE Title LIKE '%Webcam%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1516035069371-29a1b244cc32?w=800'
WHERE Title LIKE '%Camera%' OR Title LIKE '%Canon%' OR Title LIKE '%DSLR%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1587583770025-99651b96423c?w=800'
WHERE Title LIKE '%Drone%' OR Title LIKE '%DJI%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1598632640487-6ea4a60ed0c8?w=800'
WHERE Title LIKE '%Charger%' OR Title LIKE '%Power%Adapter%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1605773527852-c546a8584ea3?w=800'
WHERE Title LIKE '%Cable%' OR Title LIKE '%USB%' OR Title LIKE '%HDMI%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1586023492125-27b2c045efd7?w=800'
WHERE Title LIKE '%Stand%' OR Title LIKE '%Mount%' OR Title LIKE '%Holder%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1601784551446-20c9e07cdbdb?w=800'
WHERE Title LIKE '%Phone Case%' OR Title LIKE '%Cover%' OR Title LIKE '%Protective%Case%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1598632640487-6ea4a60ed0c8?w=800'
WHERE Title LIKE '%Powerbank%' OR Title LIKE '%Power Bank%' OR Title LIKE '%Battery%Pack%' OR Title LIKE '%Anker%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1563775341215-f1a5b2f61abc?w=800'
WHERE Title LIKE '%SSD%' OR Title LIKE '%Hard Drive%' OR Title LIKE '%Storage%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1622979135225-d2ba269cf1ac?w=800'
WHERE Title LIKE '%VR%' OR Title LIKE '%Virtual Reality%' OR Title LIKE '%Oculus%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1590602847861-f357a9332bbc?w=800'
WHERE Title LIKE '%Microphone%' OR Title LIKE '%Mic%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1559136555-9303baea8ebd?w=800'
WHERE Title LIKE '%Router%' OR Title LIKE '%WiFi%' OR Title LIKE '%Access Point%';

-- ============================================================
-- SHOES / FASHION
-- ============================================================

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=800'
WHERE Title LIKE '%Nike%' OR Title LIKE '%Sneaker%' OR Title LIKE '%Shoe%';

-- ============================================================
-- CATEGORY FALLBACK (for any remaining products without images)
-- ============================================================

UPDATE p
SET p.ImageUrl = 
  CASE 
    WHEN c.Name = 'Computers'   THEN 'https://images.unsplash.com/photo-1496181133206-80ce9b88a853?w=800'
    WHEN c.Name = 'Phone'       THEN 'https://images.unsplash.com/photo-1511707171634-5f897ff02aa9?w=800'
    WHEN c.Name = 'Audio'       THEN 'https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=800'
    WHEN c.Name = 'Wearables'   THEN 'https://images.unsplash.com/photo-1523275335684-37898b6baf30?w=800'
    WHEN c.Name = 'Gaming'      THEN 'https://images.unsplash.com/photo-1493711662062-fa541adb3fc8?w=800'
    WHEN c.Name = 'Accessories' THEN 'https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=800'
    ELSE 'https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=800'
  END
FROM dbo.Products p
INNER JOIN dbo.Categories c ON p.CategoryId = c.Id
WHERE p.ImageUrl IS NULL OR p.ImageUrl = '';

-- ============================================================
-- VERIFICATION
-- ============================================================

SELECT Id, Title, ImageUrl FROM dbo.Products ORDER BY Id;
