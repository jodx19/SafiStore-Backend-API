-- SafiStore - Update all product images with real Unsplash URLs
-- Run directly on your Monster DB
-- 70 products with specific, curated Unsplash images by category

-- ============================================================
-- COMPUTERS & LAPTOPS (15)
-- ============================================================

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1517336714460-45780111d16b?q=80&w=800'
WHERE Title LIKE '%MacBook Pro 16%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1593642632823-8f785ba67e45?q=80&w=800'
WHERE Title LIKE '%Dell XPS 15%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1603302576837-37561b2e2302?q=80&w=800'
WHERE Title LIKE '%Lenovo ThinkPad X1%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1588872657578-7efd1f1555ed?q=80&w=800'
WHERE Title LIKE '%ASUS ROG Zephyrus%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1531297172864-d541e4d00b02?q=80&w=800'
WHERE Title LIKE '%HP Spectre x360%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1595225476474-87563907a212?q=80&w=800'
WHERE Title LIKE '%Razer Blade 15%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1496181133206-80ce9b88a853?q=80&w=800'
WHERE Title LIKE '%Microsoft Surface Laptop 5%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1603302576837-37561b2e2302?q=80&w=800'
WHERE Title LIKE '%Acer Predator Helios%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1541807084-5c52b6b3adef?q=80&w=800'
WHERE Title LIKE '%LG Gram 17%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1588872657578-7efd1f1555ed?q=80&w=800'
WHERE Title LIKE '%MSI Stealth 16%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1593640408182-31c70c8268f5?q=80&w=800'
WHERE Title LIKE '%Alienware m18%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1517059224940-d4af9eec41b7?q=80&w=800'
WHERE Title LIKE '%Apple iMac 24%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1623345805615-546ec84d8583?q=80&w=800'
WHERE Title LIKE '%Mac Studio M2%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1603302576837-37561b2e2302?q=80&w=800'
WHERE Title LIKE '%Asus TUF Dash%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1593642632823-8f785ba67e45?q=80&w=800'
WHERE Title LIKE '%Gigabyte AORUS 17%';

-- ============================================================
-- SMARTPHONES (12)
-- ============================================================

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1696446701796-da61225697cc?q=80&w=800'
WHERE Title LIKE '%iPhone 15 Pro Max%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1610945265064-0e34e5519bbf?q=80&w=800'
WHERE Title LIKE '%Samsung Galaxy S24 Ultra%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1695583191199-562ec872439c?q=80&w=800'
WHERE Title LIKE '%Google Pixel 8 Pro%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1640622300473-977435c38c04?q=80&w=800'
WHERE Title LIKE '%OnePlus 12%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1678911820864-e2c567c655d7?q=80&w=800'
WHERE Title LIKE '%Nothing Phone%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1511707171634-5f897ff02aa9?q=80&w=800'
WHERE Title LIKE '%Sony Xperia 1 V%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1598327105666-5b89351aff97?q=80&w=800'
WHERE Title LIKE '%Xiaomi 14 Pro%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1585060544812-6b45742d762f?q=80&w=800'
WHERE Title LIKE '%ASUS ROG Phone 8%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1565849904461-04a58ad377e0?q=80&w=800'
WHERE Title LIKE '%Motorola Edge+%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1533228100845-08145b01de14?q=80&w=800'
WHERE Title LIKE '%Huawei P60 Pro%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1605236453806-6ff36851218e?q=80&w=800'
WHERE Title LIKE '%iPhone 13 Mini%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1580910051074-3eb694886505?q=80&w=800'
WHERE Title LIKE '%Samsung Galaxy Z Fold%';

-- ============================================================
-- AUDIO & HEADPHONES (12)
-- ============================================================

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1618366712214-8c07618ce551?q=80&w=800'
WHERE Title LIKE '%Sony WH-1000XM5%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1588423770186-80f339a7a5ad?q=80&w=800'
WHERE Title LIKE '%AirPods Pro%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1546435770-a3e426da4717?q=80&w=800'
WHERE Title LIKE '%Bose QuietComfort Ultra%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1613040809024-b4ef7ba99bc3?q=80&w=800'
WHERE Title LIKE '%AirPods Max%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1583394838336-acd977736f90?q=80&w=800'
WHERE Title LIKE '%Sennheiser Momentum 4%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1505740420928-5e560c06d30e?q=80&w=800'
WHERE Title LIKE '%Beats Studio Pro%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1590658268037-6bf12165a8df?q=80&w=800'
WHERE Title LIKE '%JBL Flip 6%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1545454675-3531b543be5d?q=80&w=800'
WHERE Title LIKE '%Marshall Stanmore%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1608043152269-423dbba4e7e1?q=80&w=800'
WHERE Title LIKE '%Sonos Roam%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1590602847861-f357a9332bbc?q=80&w=800'
WHERE Title LIKE '%Jabra Elite 8%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1598488035139-bdbb2231ce04?q=80&w=800'
WHERE Title LIKE '%Shure SM7B%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1586716402243-7b3e15f60eb3?q=80&w=800'
WHERE Title LIKE '%Blue Yeti%';

-- ============================================================
-- WEARABLES & TABLETS (10)
-- ============================================================

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1546868871-7041f2a55e12?q=80&w=800'
WHERE Title LIKE '%Apple Watch Ultra 2%' OR Title LIKE '%Apple Watch Ultra%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1434493789847-2f02dc6ca35d?q=80&w=800'
WHERE Title LIKE '%Apple Watch Series 9%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1508685096489-7aacd43bd3b1?q=80&w=800'
WHERE Title LIKE '%Samsung Galaxy Watch 6%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1523275335684-37898b6baf30?q=80&w=800'
WHERE Title LIKE '%Garmin Fenix 7%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1579586337278-3befd40fd17a?q=80&w=800'
WHERE Title LIKE '%Google Pixel Watch 2%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1544244015-0df4b3ffc6b0?q=80&w=800'
WHERE Title LIKE '%iPad Pro 12.9%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1589739900243-4b52cd9b104e?q=80&w=800'
WHERE Title LIKE '%Samsung Galaxy Tab S9%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1593305841991-05c297ba4575?q=80&w=800'
WHERE Title LIKE '%Microsoft Surface Pro 9%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1592434134753-a70baf7979d7?q=80&w=800'
WHERE Title LIKE '%Amazon Kindle%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1542751110-97427bbecf20?q=80&w=800'
WHERE Title LIKE '%Lenovo Tab P12%';

-- ============================================================
-- GAMING & PERIPHERALS (11)
-- ============================================================

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1606813907291-d86ebb995a26?q=80&w=800'
WHERE Title LIKE '%PlayStation 5%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1621259182978-fbf93132d53d?q=80&w=800'
WHERE Title LIKE '%Xbox Series X%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1578303512597-81e6cc155b3e?q=80&w=800'
WHERE Title LIKE '%Nintendo Switch%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1640552435532-67d710bf3364?q=80&w=800'
WHERE Title LIKE '%Steam Deck%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1605901309584-818e25960b8f?q=80&w=800'
WHERE Title LIKE '%ASUS ROG Ally%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1615663245857-ac93bb7c39e7?q=80&w=800'
WHERE Title LIKE '%Logitech MX Master 3S%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1595225476474-87563907a212?q=80&w=800'
WHERE Title LIKE '%Razer DeathAdder%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1511467687858-23d96c32e4ae?q=80&w=800'
WHERE Title LIKE '%Keychron Q1%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1595225476474-87563907a212?q=80&w=800'
WHERE Title LIKE '%SteelSeries Apex%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1593305841991-05c297ba4575?q=80&w=800'
WHERE Title LIKE '%Elgato Stream Deck%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1527814050087-379381547928?q=80&w=800'
WHERE Title LIKE '%Logitech G Pro X Superlight%';

-- ============================================================
-- CAMERAS, MONITORS & OFFICE (10)
-- ============================================================

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1516035069371-29a1b244cc32?q=80&w=800'
WHERE Title LIKE '%Sony A7 IV%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1502920917128-1aa500764cbd?q=80&w=800'
WHERE Title LIKE '%Canon EOS R6%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1520390138845-fd2d229dd553?q=80&w=800'
WHERE Title LIKE '%GoPro HERO12%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1473966968600-fa801b869a1a?q=80&w=800'
WHERE Title LIKE '%DJI Mini 4 Pro%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1527443224154-c4a3942d3acf?q=80&w=800'
WHERE Title LIKE '%LG UltraGear%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1542751110-97427bbecf20?q=80&w=800'
WHERE Title LIKE '%Samsung Odyssey G9%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1505797149-43b0069ec26b?q=80&w=800'
WHERE Title LIKE '%Secretlab Titan%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1595515106969-1ce29566ff1c?q=80&w=800'
WHERE Title LIKE '%ErgoTune Supreme%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1563291074-2bf8677ac0e5?q=80&w=800'
WHERE Title LIKE '%SanDisk 2TB Extreme%';

UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1550985543-f47f52726384?q=80&w=800'
WHERE Title LIKE '%Philips Hue%';

-- ============================================================
-- VERIFICATION
-- ============================================================

SELECT '=== PRODUCTS WITH IMAGES ===' AS '';
SELECT Id, Title, ImageUrl FROM dbo.Products ORDER BY Id;

SELECT '=== PRODUCTS WITHOUT IMAGES ===' AS '';
SELECT Id, Title FROM dbo.Products WHERE ImageUrl IS NULL OR ImageUrl = '' ORDER BY Id;
