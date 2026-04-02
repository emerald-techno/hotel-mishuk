--Division Entry Query
INSERT INTO SetDivisions 
    (Name, NameBn, Code, CountryId, ActionById, ActionDate, UpdatedById, UpdateDate, IsDeleted)
VALUES
    ('Dhaka',        N'ঢাকা',         'DHA', 1, 2, GETDATE(), 2, GETDATE(), 0),
    ('Chattogram',   N'চট্টগ্রাম',    'CTG', 1, 2, GETDATE(), 2, GETDATE(), 0),
    ('Rajshahi',     N'রাজশাহী',      'RAJ', 1, 2, GETDATE(), 2, GETDATE(), 0),
    ('Khulna',       N'খুলনা',        'KHU', 1, 2, GETDATE(), 2, GETDATE(), 0),
    ('Barishal',     N'বরিশাল',       'BAR', 1, 2, GETDATE(), 2, GETDATE(), 0),
    ('Sylhet',       N'সিলেট',        'SYL', 1, 2, GETDATE(), 2, GETDATE(), 0),
    ('Rangpur',      N'রংপুর',        'RAN', 1, 2, GETDATE(), 2, GETDATE(), 0),
    ('Mymensingh',   N'ময়মনসিংহ',     'MYS', 1, 2, GETDATE(), 2, GETDATE(), 0);

--Districs Add Query
INSERT INTO SetDistricts 
(Name, NameBn, Code, DivisionId, ActionById, ActionDate, UpdatedById, UpdateDate, IsDeleted)
VALUES
-- Dhaka Division (1)
('Dhaka', N'ঢাকা', 'DHK', 1, 2, GETDATE(), 2, GETDATE(), 0),
('Gazipur', N'গাজীপুর', 'GAZ', 1, 2, GETDATE(), 2, GETDATE(), 0),
('Kishoreganj', N'কিশোরগঞ্জ', 'KIS', 1, 2, GETDATE(), 2, GETDATE(), 0),
('Manikganj', N'মানিকগঞ্জ', 'MAN', 1, 2, GETDATE(), 2, GETDATE(), 0),
('Munshiganj', N'মুন্সীগঞ্জ', 'MUN', 1, 2, GETDATE(), 2, GETDATE(), 0),
('Narayanganj', N'নারায়ণগঞ্জ', 'NAR', 1, 2, GETDATE(), 2, GETDATE(), 0),
('Narsingdi', N'নরসিংদী', 'NAS', 1, 2, GETDATE(), 2, GETDATE(), 0),
('Tangail', N'টাঙ্গাইল', 'TAN', 1, 2, GETDATE(), 2, GETDATE(), 0),
('Faridpur', N'ফরিদপুর', 'FAR', 1, 2, GETDATE(), 2, GETDATE(), 0),
('Gopalganj', N'গোপালগঞ্জ', 'GOP', 1, 2, GETDATE(), 2, GETDATE(), 0),
('Madaripur', N'মাদারীপুর', 'MAD', 1, 2, GETDATE(), 2, GETDATE(), 0),
('Rajbari', N'রাজবাড়ী', 'RAJ', 1, 2, GETDATE(), 2, GETDATE(), 0),
('Shariatpur', N'শরিয়তপুর', 'SHA', 1, 2, GETDATE(), 2, GETDATE(), 0),

-- Chattogram Division (2)
('Chattogram', N'চট্টগ্রাম', 'CTG', 2, 2, GETDATE(), 2, GETDATE(), 0),
('Cox''s Bazar', N'কক্সবাজার', 'COX', 2, 2, GETDATE(), 2, GETDATE(), 0),
('Cumilla', N'কুমিল্লা', 'CUM', 2, 2, GETDATE(), 2, GETDATE(), 0),
('Brahmanbaria', N'ব্রাহ্মণবাড়িয়া', 'BRA', 2, 2, GETDATE(), 2, GETDATE(), 0),
('Chandpur', N'চাঁদপুর', 'CHA', 2, 2, GETDATE(), 2, GETDATE(), 0),
('Feni', N'ফেনী', 'FEN', 2, 2, GETDATE(), 2, GETDATE(), 0),
('Lakshmipur', N'লক্ষ্মীপুর', 'LAK', 2, 2, GETDATE(), 2, GETDATE(), 0),
('Noakhali', N'নোয়াখালী', 'NOA', 2, 2, GETDATE(), 2, GETDATE(), 0),
('Khagrachhari', N'খাগড়াছড়ি', 'KHA', 2, 2, GETDATE(), 2, GETDATE(), 0),
('Rangamati', N'রাঙ্গামাটি', 'RAN', 2, 2, GETDATE(), 2, GETDATE(), 0),
('Bandarban', N'বান্দরবান', 'BAN', 2, 2, GETDATE(), 2, GETDATE(), 0),

-- Rajshahi Division (3)
('Rajshahi', N'রাজশাহী', 'RAJ', 3, 2, GETDATE(), 2, GETDATE(), 0),
('Natore', N'নাটোর', 'NAT', 3, 2, GETDATE(), 2, GETDATE(), 0),
('Chapainawabganj', N'চাঁপাইনবাবগঞ্জ', 'CHA', 3, 2, GETDATE(), 2, GETDATE(), 0),
('Pabna', N'পাবনা', 'PAB', 3, 2, GETDATE(), 2, GETDATE(), 0),
('Sirajganj', N'সিরাজগঞ্জ', 'SIR', 3, 2, GETDATE(), 2, GETDATE(), 0),
('Bogura', N'বগুড়া', 'BOG', 3, 2, GETDATE(), 2, GETDATE(), 0),
('Joypurhat', N'জয়পুরহাট', 'JOY', 3, 2, GETDATE(), 2, GETDATE(), 0),

-- Khulna Division (4)
('Khulna', N'খুলনা', 'KHU', 4, 2, GETDATE(), 2, GETDATE(), 0),
('Bagerhat', N'বাগেরহাট', 'BAG', 4, 2, GETDATE(), 2, GETDATE(), 0),
('Satkhira', N'সাতক্ষীরা', 'SAT', 4, 2, GETDATE(), 2, GETDATE(), 0),
('Jessore', N'যশোর', 'JES', 4, 2, GETDATE(), 2, GETDATE(), 0),
('Jhenaidah', N'ঝিনাইদহ', 'JHE', 4, 2, GETDATE(), 2, GETDATE(), 0),
('Kushtia', N'কুষ্টিয়া', 'KUS', 4, 2, GETDATE(), 2, GETDATE(), 0),
('Magura', N'মাগুরা', 'MAG', 4, 2, GETDATE(), 2, GETDATE(), 0),
('Meherpur', N'মেহেরপুর', 'MEH', 4, 2, GETDATE(), 2, GETDATE(), 0),
('Narail', N'নড়াইল', 'NAR', 4, 2, GETDATE(), 2, GETDATE(), 0),
('Chuadanga', N'চুয়াডাঙ্গা', 'CHU', 4, 2, GETDATE(), 2, GETDATE(), 0),

-- Barishal Division (5)
('Barishal', N'বরিশাল', 'BAR', 5, 2, GETDATE(), 2, GETDATE(), 0),
('Patuakhali', N'পটুয়াখালী', 'PAT', 5, 2, GETDATE(), 2, GETDATE(), 0),
('Pirojpur', N'পিরোজপুর', 'PIR', 5, 2, GETDATE(), 2, GETDATE(), 0),
('Bhola', N'ভোলা', 'BHO', 5, 2, GETDATE(), 2, GETDATE(), 0),
('Barguna', N'বরগুনা', 'BAG', 5, 2, GETDATE(), 2, GETDATE(), 0),
('Jhalokathi', N'ঝালকাঠি', 'JHA', 5, 2, GETDATE(), 2, GETDATE(), 0),

-- Sylhet Division (6)
('Sylhet', N'সিলেট', 'SYL', 6, 2, GETDATE(), 2, GETDATE(), 0),
('Moulvibazar', N'মৌলভীবাজার', 'MOU', 6, 2, GETDATE(), 2, GETDATE(), 0),
('Habiganj', N'হবিগঞ্জ', 'HAB', 6, 2, GETDATE(), 2, GETDATE(), 0),
('Sunamganj', N'সুনামগঞ্জ', 'SUN', 6, 2, GETDATE(), 2, GETDATE(), 0),

-- Rangpur Division (7)
('Rangpur', N'রংপুর', 'RAN', 7, 2, GETDATE(), 2, GETDATE(), 0),
('Kurigram', N'কুড়িগ্রাম', 'KUR', 7, 2, GETDATE(), 2, GETDATE(), 0),
('Lalmonirhat', N'লালমনিরহাট', 'LAL', 7, 2, GETDATE(), 2, GETDATE(), 0),
('Nilphamari', N'নীলফামারী', 'NIL', 7, 2, GETDATE(), 2, GETDATE(), 0),
('Gaibandha', N'গাইবান্ধা', 'GAI', 7, 2, GETDATE(), 2, GETDATE(), 0),
('Thakurgaon', N'ঠাকুরগাঁও', 'THA', 7, 2, GETDATE(), 2, GETDATE(), 0),
('Dinajpur', N'দিনাজপুর', 'DIN', 7, 2, GETDATE(), 2, GETDATE(), 0),
('Panchagarh', N'পঞ্চগড়', 'PAN', 7, 2, GETDATE(), 2, GETDATE(), 0),

-- Mymensingh Division (8)
('Mymensingh', N'ময়মনসিংহ', 'MYS', 8, 2, GETDATE(), 2, GETDATE(), 0),
('Netrokona', N'নেত্রকোণা', 'NET', 8, 2, GETDATE(), 2, GETDATE(), 0),
('Sherpur', N'শেরপুর', 'SHE', 8, 2, GETDATE(), 2, GETDATE(), 0),
('Jamalpur', N'জামালপুর', 'JAM', 8, 2, GETDATE(), 2, GETDATE(), 0);
