using HotelBookingSystem.Models;
using HotelBookingSystem.Services;

namespace HotelBookingSystem.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Hotels.Any())
            {
                return; // DB has been seeded
            }

            // 1. Seed Users
            var adminUser = new AppUser
            {
                Username = "admin",
                Email = "admin@hotel.com",
                FullName = "Quản Trị Viên Khách Sạn",
                Phone = "0988888888",
                Role = "Admin",
                PasswordHash = SecurityHelper.HashPassword("Admin@123456"),
                CreatedAt = DateTime.Now
            };

            var customerUser = new AppUser
            {
                Username = "customer",
                Email = "khachhang@gmail.com",
                FullName = "Nguyễn Văn An",
                Phone = "0912345678",
                Role = "Customer",
                PasswordHash = SecurityHelper.HashPassword("User@123456"),
                CreatedAt = DateTime.Now
            };

            context.AppUsers.AddRange(adminUser, customerUser);
            context.SaveChanges();

            // 2. Seed Extra Services
            var services = new List<ExtraService>
            {
                new ExtraService
                {
                    Name = "Bữa sáng Buffet Á - Âu thịnh soạn",
                    Description = "Bữa sáng buffet hơn 40 món đặc sản vùng miền và đồ ăn Âu, cà phê pha máy",
                    Price = 120000,
                    Unit = "người / ngày",
                    Icon = "bi-cup-hot",
                    IsActive = true
                },
                new ExtraService
                {
                    Name = "Đưa đón sân bay 2 chiều xe riêng",
                    Description = "Xe du lịch đời mới 7 chỗ, tài xế đón tận nơi với biển tên khách sạn",
                    Price = 450000,
                    Unit = "chuyến",
                    Icon = "bi-car-front",
                    IsActive = true
                },
                new ExtraService
                {
                    Name = "Thuê xe máy tay ga (24h)",
                    Description = "Xe Honda Vision / AirBlade đời mới, kèm 2 mũ bảo hiểm và áo mưa",
                    Price = 150000,
                    Unit = "ngày / xe",
                    Icon = "bi-bicycle",
                    IsActive = true
                },
                new ExtraService
                {
                    Name = "Set tiệc BBQ sân vườn & Bếp than hoa",
                    Description = "Trang bị đầy đủ bếp nướng, than củi không khói, chén đĩa và gia vị ướp",
                    Price = 300000,
                    Unit = "set / lượt",
                    Icon = "bi-fire",
                    IsActive = true
                },
                new ExtraService
                {
                    Name = "Tour trải nghiệm & Săn mây / Lặn biển ngắm san hô",
                    Description = "Có hướng dẫn viên bản địa am hiểu, bao gồm nước suối và thiết bị an toàn",
                    Price = 400000,
                    Unit = "khách",
                    Icon = "bi-compass",
                    IsActive = true
                },
                new ExtraService
                {
                    Name = "Giặt là sấy thơm lấy ngay trong 4 tiếng",
                    Description = "Quần áo được phân loại giặt sấy cao cấp, ủi phẳng gấp gọn",
                    Price = 50000,
                    Unit = "kg",
                    Icon = "bi-bag-check",
                    IsActive = true
                }
            };

            context.ExtraServices.AddRange(services);
            context.SaveChanges();

            // 3. Seed Hotels & Homestays
            var dalatHotel = new Hotel
            {
                Name = "Đà Lạt Pine Forest Cozy Homestay",
                City = "Đà Lạt",
                Address = "Số 18 Đường Khởi Nghĩa Bắc Sơn, Phường 10, TP. Đà Lạt",
                Description = "Homestay phong cách mộc mạc ẩn mình giữa rừng thông xanh mát, không gian lãng mạn, lò sưởi ấm cúng và vườn hoa cẩm tú cầu rực rỡ.",
                StarRating = 4,
                Phone = "02633888999",
                Email = "booking@dalatpinehomestay.vn",
                ImageUrl = "https://images.unsplash.com/photo-1566073771259-6a8506099945?auto=format&fit=crop&w=1200&q=80"
            };

            var phuquocHotel = new Hotel
            {
                Name = "Phú Quốc Emerald Bay Luxury Resort & Villas",
                City = "Phú Quốc",
                Address = "Bãi Khem, Phường An Thới, TP. Phú Quốc, Kiên Giang",
                Description = "Khu nghỉ dưỡng 5 sao sát bờ biển cát trắng mịn như kem, hồ bơi vô cực view hoàng hôn biển Tây và nhà hàng hải sản tươi sống thượng hạng.",
                StarRating = 5,
                Phone = "02973999888",
                Email = "res@phuquocemeraldbay.vn",
                ImageUrl = "https://images.unsplash.com/photo-1582719508461-905c673771fd?auto=format&fit=crop&w=1200&q=80"
            };

            var danangHotel = new Hotel
            {
                Name = "Đà Nẵng Blue Horizon Oceanview Hotel",
                City = "Đà Nẵng",
                Address = "Số 120 Đường Võ Nguyên Giáp, Phước Mỹ, Sơn Trà, TP. Đà Nẵng",
                Description = "Khách sạn hiện đại chỉ cách bãi biển Mỹ Khê 2 phút đi bộ, hồ bơi sân thượng tầng 25 ngắm trọn biển Đông và cầu Rồng lung linh về đêm.",
                StarRating = 4,
                Phone = "02363777666",
                Email = "contact@bluehorizondanang.com",
                ImageUrl = "https://images.unsplash.com/photo-1542314831-068cd1dbfeeb?auto=format&fit=crop&w=1200&q=80"
            };

            var hanoiHotel = new Hotel
            {
                Name = "Hà Nội Old Quarter Charm Boutique",
                City = "Hà Nội",
                Address = "Số 36 Phố Mã Mây, Hàng Buồm, Hoàn Kiếm, Hà Nội",
                Description = "Khách sạn mang phong cách Indochine Đông Dương sang trọng, nằm ngay trái tim phố cổ 36 phố phường, tiện lợi dạo phố đi bộ hồ Gươm.",
                StarRating = 4,
                Phone = "02438881234",
                Email = "info@hanoicharmboutique.vn",
                ImageUrl = "https://images.unsplash.com/photo-1571896349842-33c89424de2d?auto=format&fit=crop&w=1200&q=80"
            };

            var sapaHotel = new Hotel
            {
                Name = "Sapa Mist Mountain Eco Lodge",
                City = "Sa Pa",
                Address = "Thôn Tả Van Giáy, Xã Tả Van, Thị xã Sa Pa, Lào Cai",
                Description = "Lodge sinh thái tuyệt đẹp hướng trọn thung lũng Mường Hoa và ruộng bậc thang kỳ vĩ, sáng ngắm biển mây bồng bềnh, tối ngắm sao trời.",
                StarRating = 4,
                Phone = "02143666888",
                Email = "stay@sapamistlodge.com",
                ImageUrl = "https://images.unsplash.com/photo-1520250497591-112f2f40a3f4?auto=format&fit=crop&w=1200&q=80"
            };

            context.Hotels.AddRange(dalatHotel, phuquocHotel, danangHotel, hanoiHotel, sapaHotel);
            context.SaveChanges();

            // 4. Seed Room Types
            var roomTypes = new List<RoomType>
            {
                // Đà Lạt
                new RoomType
                {
                    HotelId = dalatHotel.Id,
                    Name = "Phòng Gác Mái Vintage View Đồi Thông",
                    Description = "Căn phòng gác mái gỗ ấm cúng với cửa sổ kính lớn phóng tầm mắt ra đồi thông bạt ngàn, ban công riêng thưởng thức trà hoa và cà phê sáng.",
                    Capacity = 2,
                    BasePrice = 650000,
                    SizeM2 = 28,
                    BedType = "1 Giường Queen Đôi Ấm Áp",
                    Amenities = "Wifi tốc độ cao, Ban công ngắm đồi thông, Máy sưởi ấm, Máy sấy tóc, Ấm đun siêu tốc, Trà & Cà phê miễn phí",
                    ImageUrl = "https://images.unsplash.com/photo-1590490360182-c33d57733427?auto=format&fit=crop&w=1000&q=80"
                },
                new RoomType
                {
                    HotelId = dalatHotel.Id,
                    Name = "Phòng Gia Đình Garden Suite Có Bếp Nhỏ",
                    Description = "Không gian rộng rãi cho cả gia đình hoặc nhóm bạn thân, có bếp nhỏ nấu nướng nhẹ nhàng và sân vườn nướng BBQ riêng biệt.",
                    Capacity = 4,
                    BasePrice = 1200000,
                    SizeM2 = 50,
                    BedType = "2 Giường King Size Cỡ Lớn",
                    Amenities = "Bếp nấu gia đình, Tủ lạnh mini, Smart TV 55 inch, Bồn tắm nằm, Máy sưởi gốm, Ban công sân vườn",
                    ImageUrl = "https://images.unsplash.com/photo-1618773928121-c32242e63f39?auto=format&fit=crop&w=1000&q=80"
                },

                // Phú Quốc
                new RoomType
                {
                    HotelId = phuquocHotel.Id,
                    Name = "Biệt Thự Oceanfront Villa Hồ Bơi Riêng",
                    Description = "Villa cao cấp bậc nhất trước biển với hồ bơi vô cực riêng tư, phòng khách rộng sang trọng và lối đi thẳng ra bờ cát trắng mịn.",
                    Capacity = 4,
                    BasePrice = 4500000,
                    SizeM2 = 120,
                    BedType = "2 Giường King Size Hạng Sang",
                    Amenities = "Hồ bơi vô cực riêng, View biển 180 độ, Quầy bar mini, Bồn sục Jacuzzi, Phòng thay đồ walk-in, Dịch vụ phòng 24/7",
                    ImageUrl = "https://images.unsplash.com/photo-1613977257363-707ba9348227?auto=format&fit=crop&w=1000&q=80"
                },
                new RoomType
                {
                    HotelId = phuquocHotel.Id,
                    Name = "Phòng Deluxe Ocean View Ban Công Rộng",
                    Description = "Phòng nghỉ hiện đại ngập tràn ánh nắng và gió biển Tây, thiết kế mở đón trọn khung cảnh hoàng hôn tím ngoạn mục.",
                    Capacity = 2,
                    BasePrice = 1850000,
                    SizeM2 = 42,
                    BedType = "1 Giường King Cực Lớn",
                    Amenities = "Ban công ngắm biển, Bồn tắm ngâm sâu, Máy pha cafe Nespresso, Áo choàng lụa, Két sắt an toàn",
                    ImageUrl = "https://images.unsplash.com/photo-1591088398332-8a7791972843?auto=format&fit=crop&w=1000&q=80"
                },

                // Đà Nẵng
                new RoomType
                {
                    HotelId = danangHotel.Id,
                    Name = "Phòng Premier Hướng Biển Mỹ Khê",
                    Description = "Tọa lạc tại tầng cao ngắm trọn vịnh Đà Nẵng và bán đảo Sơn Trà, nội thất phong cách Bắc Âu thanh lịch và trang nhã.",
                    Capacity = 2,
                    BasePrice = 980000,
                    SizeM2 = 35,
                    BedType = "1 Giường King hoặc 2 Giường Đơn",
                    Amenities = "Cửa sổ kính kịch trần view biển, Bồn tắm kính hiện đại, Bàn làm việc, Smart TV Netflix, Tủ lạnh mini",
                    ImageUrl = "https://images.unsplash.com/photo-1566665797739-1674de7a421a?auto=format&fit=crop&w=1000&q=80"
                },
                new RoomType
                {
                    HotelId = danangHotel.Id,
                    Name = "Phòng Executive Family Suite Cầu Rồng",
                    Description = "Căn hộ suite gia đình gồm 2 phòng ngủ riêng biệt và phòng khách ngắm pháo hoa và cảnh đêm cầu Rồng huyền ảo.",
                    Capacity = 5,
                    BasePrice = 1750000,
                    SizeM2 = 65,
                    BedType = "1 Giường King + 2 Giường Đơn",
                    Amenities = "2 Phòng ngủ độc lập, 2 Phòng vệ sinh, Bàn ăn, Sofa da cao cấp, Ban công hóng mát thành phố",
                    ImageUrl = "https://images.unsplash.com/photo-1578683010236-d716f9a3f461?auto=format&fit=crop&w=1000&q=80"
                },

                // Hà Nội
                new RoomType
                {
                    HotelId = hanoiHotel.Id,
                    Name = "Phòng Heritage Boutique Phố Cổ",
                    Description = "Đậm nét thanh lịch văn hóa Tràng An, gạch bông cổ điển, quạt trần gỗ và nội thất Indochine hoài niệm giữa lòng phố cổ náo nhiệt.",
                    Capacity = 2,
                    BasePrice = 850000,
                    SizeM2 = 32,
                    BedType = "1 Giường Đôi Cổ Điển",
                    Amenities = "Cửa sổ gỗ ngắm phố, Trà sen Hà Nội, Bồn tắm phong cách retro, Cách âm cao cấp, Máy lọc không khí",
                    ImageUrl = "https://images.unsplash.com/photo-1595526114035-0d45ed16cfbf?auto=format&fit=crop&w=1000&q=80"
                },

                // Sa Pa
                new RoomType
                {
                    HotelId = sapaHotel.Id,
                    Name = "Phòng Panoramic Săn Mây Mường Hoa",
                    Description = "Mỗi buổi sáng thức dậy như lơ lửng trên tầng mây, ban công gỗ hướng thẳng dãy Hoàng Liên Sơn và đỉnh Fansipan hùng vĩ.",
                    Capacity = 2,
                    BasePrice = 1150000,
                    SizeM2 = 38,
                    BedType = "1 Giường King Trùm Chăn Thổ Cẩm",
                    Amenities = "Ban công săn mây, Lò sưởi củi truyền thống, Bồn tắm gỗ Pơ-mu thảo dược, Trà thảo mộc bản địa",
                    ImageUrl = "https://images.unsplash.com/photo-1506744038136-46273834b3fb?auto=format&fit=crop&w=1000&q=80"
                }
            };

            context.RoomTypes.AddRange(roomTypes);
            context.SaveChanges();

            // 5. Seed Physical Rooms
            var rooms = new List<Room>();

            // Đà Lạt - Gác Mái (3 rooms)
            rooms.Add(new Room { RoomNumber = "DL-101", Floor = 1, RoomTypeId = roomTypes[0].Id, HotelId = dalatHotel.Id, Status = RoomStatus.Available });
            rooms.Add(new Room { RoomNumber = "DL-102", Floor = 1, RoomTypeId = roomTypes[0].Id, HotelId = dalatHotel.Id, Status = RoomStatus.Available });
            rooms.Add(new Room { RoomNumber = "DL-201", Floor = 2, RoomTypeId = roomTypes[0].Id, HotelId = dalatHotel.Id, Status = RoomStatus.Available });

            // Đà Lạt - Family Suite (2 rooms)
            rooms.Add(new Room { RoomNumber = "DL-G01", Floor = 1, RoomTypeId = roomTypes[1].Id, HotelId = dalatHotel.Id, Status = RoomStatus.Available });
            rooms.Add(new Room { RoomNumber = "DL-G02", Floor = 1, RoomTypeId = roomTypes[1].Id, HotelId = dalatHotel.Id, Status = RoomStatus.Available });

            // Phú Quốc - Oceanfront Villa (2 rooms)
            rooms.Add(new Room { RoomNumber = "VILLA-01", Floor = 1, RoomTypeId = roomTypes[2].Id, HotelId = phuquocHotel.Id, Status = RoomStatus.Available });
            rooms.Add(new Room { RoomNumber = "VILLA-02", Floor = 1, RoomTypeId = roomTypes[2].Id, HotelId = phuquocHotel.Id, Status = RoomStatus.Available });

            // Phú Quốc - Deluxe Ocean View (3 rooms)
            rooms.Add(new Room { RoomNumber = "PQ-301", Floor = 3, RoomTypeId = roomTypes[3].Id, HotelId = phuquocHotel.Id, Status = RoomStatus.Available });
            rooms.Add(new Room { RoomNumber = "PQ-302", Floor = 3, RoomTypeId = roomTypes[3].Id, HotelId = phuquocHotel.Id, Status = RoomStatus.Available });
            rooms.Add(new Room { RoomNumber = "PQ-401", Floor = 4, RoomTypeId = roomTypes[3].Id, HotelId = phuquocHotel.Id, Status = RoomStatus.Available });

            // Đà Nẵng - Premier Ocean (3 rooms)
            rooms.Add(new Room { RoomNumber = "DN-501", Floor = 5, RoomTypeId = roomTypes[4].Id, HotelId = danangHotel.Id, Status = RoomStatus.Available });
            rooms.Add(new Room { RoomNumber = "DN-502", Floor = 5, RoomTypeId = roomTypes[4].Id, HotelId = danangHotel.Id, Status = RoomStatus.Available });
            rooms.Add(new Room { RoomNumber = "DN-601", Floor = 6, RoomTypeId = roomTypes[4].Id, HotelId = danangHotel.Id, Status = RoomStatus.Maintenance });

            // Đà Nẵng - Executive Family (2 rooms)
            rooms.Add(new Room { RoomNumber = "DN-701", Floor = 7, RoomTypeId = roomTypes[5].Id, HotelId = danangHotel.Id, Status = RoomStatus.Available });
            rooms.Add(new Room { RoomNumber = "DN-702", Floor = 7, RoomTypeId = roomTypes[5].Id, HotelId = danangHotel.Id, Status = RoomStatus.Available });

            // Hà Nội - Heritage (3 rooms)
            rooms.Add(new Room { RoomNumber = "HN-201", Floor = 2, RoomTypeId = roomTypes[6].Id, HotelId = hanoiHotel.Id, Status = RoomStatus.Available });
            rooms.Add(new Room { RoomNumber = "HN-202", Floor = 2, RoomTypeId = roomTypes[6].Id, HotelId = hanoiHotel.Id, Status = RoomStatus.Available });
            rooms.Add(new Room { RoomNumber = "HN-301", Floor = 3, RoomTypeId = roomTypes[6].Id, HotelId = hanoiHotel.Id, Status = RoomStatus.Available });

            // Sa Pa - Panoramic (3 rooms)
            rooms.Add(new Room { RoomNumber = "SP-101", Floor = 1, RoomTypeId = roomTypes[7].Id, HotelId = sapaHotel.Id, Status = RoomStatus.Available });
            rooms.Add(new Room { RoomNumber = "SP-102", Floor = 1, RoomTypeId = roomTypes[7].Id, HotelId = sapaHotel.Id, Status = RoomStatus.Available });
            rooms.Add(new Room { RoomNumber = "SP-201", Floor = 2, RoomTypeId = roomTypes[7].Id, HotelId = sapaHotel.Id, Status = RoomStatus.Available });

            context.Rooms.AddRange(rooms);
            context.SaveChanges();

            // 6. Seed Sample Bookings
            var today = DateTime.Today;
            var sampleBooking1 = new Booking
            {
                BookingCode = "BK20261101901",
                CustomerName = "Trần Minh Hoàng",
                CustomerEmail = "hoang.tm@gmail.com",
                CustomerPhone = "0905123456",
                CheckInDate = today.AddDays(-2),
                CheckOutDate = today.AddDays(1),
                TotalNights = 3,
                GuestsCount = 2,
                RoomId = rooms[0].Id, // DL-101
                RoomPrice = 650000 * 3,
                ServicesPrice = 240000,
                TotalAmount = (650000 * 3) + 240000,
                Status = BookingStatus.CheckedIn,
                PaymentStatus = PaymentStatus.Paid,
                PaymentMethod = "VNPay",
                CreatedAt = today.AddDays(-5),
                SpecialRequests = "Check-in sớm lúc 12:00, chuẩn bị thêm gối lông vũ"
            };

            var sampleBooking2 = new Booking
            {
                BookingCode = "BK20261103854",
                CustomerName = "Lê Thị Ngọc Mai",
                CustomerEmail = "ngocmai.le@yahoo.com",
                CustomerPhone = "0938555666",
                CheckInDate = today,
                CheckOutDate = today.AddDays(2),
                TotalNights = 2,
                GuestsCount = 2,
                RoomId = rooms[5].Id, // VILLA-01
                RoomPrice = 4500000 * 2,
                ServicesPrice = 900000,
                TotalAmount = (4500000 * 2) + 900000,
                Status = BookingStatus.Confirmed,
                PaymentStatus = PaymentStatus.Paid,
                PaymentMethod = "VietQR",
                CreatedAt = today.AddDays(-2),
                SpecialRequests = "Kỷ niệm ngày cưới, set up hoa tươi trên giường ngủ"
            };

            var sampleBooking3 = new Booking
            {
                BookingCode = "BK20261104112",
                CustomerName = "Đặng Vũ Hải",
                CustomerEmail = "haidv@fpt.com",
                CustomerPhone = "0977888999",
                CheckInDate = today.AddDays(3),
                CheckOutDate = today.AddDays(5),
                TotalNights = 2,
                GuestsCount = 2,
                RoomId = rooms[9].Id, // DN-501
                RoomPrice = 980000 * 2,
                ServicesPrice = 300000,
                TotalAmount = (980000 * 2) + 300000,
                Status = BookingStatus.Pending,
                PaymentStatus = PaymentStatus.Unpaid,
                PaymentMethod = "Cash",
                CreatedAt = today,
                SpecialRequests = "Yêu cầu phòng tầng cao view thoáng không hút thuốc"
            };

            context.Bookings.AddRange(sampleBooking1, sampleBooking2, sampleBooking3);
            context.SaveChanges();

            // Seed BookingServices
            context.BookingServices.AddRange(
                new BookingExtraService
                {
                    BookingId = sampleBooking1.Id,
                    ServiceId = services[0].Id, // Buffet
                    Quantity = 2,
                    UnitPrice = services[0].Price,
                    TotalPrice = services[0].Price * 2
                },
                new BookingExtraService
                {
                    BookingId = sampleBooking2.Id,
                    ServiceId = services[1].Id, // Đưa đón sân bay
                    Quantity = 2,
                    UnitPrice = services[1].Price,
                    TotalPrice = services[1].Price * 2
                },
                new BookingExtraService
                {
                    BookingId = sampleBooking3.Id,
                    ServiceId = services[2].Id, // Thuê xe máy
                    Quantity = 2,
                    UnitPrice = services[2].Price,
                    TotalPrice = services[2].Price * 2
                }
            );

            context.SaveChanges();
        }
    }
}
