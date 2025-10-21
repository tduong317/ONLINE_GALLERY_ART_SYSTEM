using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gallery_Art_System.Migrations
{
    public partial class CreateBannerTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Admin",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admin", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    category_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.category_id);
                });

            migrationBuilder.CreateTable(
                name: "Config",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Mail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MailPort = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MailInfo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Contact = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Hotline = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SocialLink1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SocialLink2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SocialLink3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LinkVideo1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LinkVideo2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LinkVideo3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Config", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Contact",
                columns: table => new
                {
                    message_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    email = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    subject = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    message = table.Column<string>(type: "text", nullable: false),
                    sent_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__contact___0BBF6EE61D7F359C", x => x.message_id);
                });

            migrationBuilder.CreateTable(
                name: "Exhibition",
                columns: table => new
                {
                    exhibition_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    location = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    start_date = table.Column<DateTime>(type: "date", nullable: true),
                    end_date = table.Column<DateTime>(type: "date", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    image_url = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exhibition", x => x.exhibition_id);
                });

            migrationBuilder.CreateTable(
                name: "Page",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Tag = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Detail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Level = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Keyword = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: true),
                    Link = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Target = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Index = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    Ord = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Page", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    password = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    full_name = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    gender = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    age = table.Column<int>(type: "int", nullable: true),
                    email = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    phone = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    address = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "Artwork",
                columns: table => new
                {
                    artwork_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    artist_id = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    category_id = table.Column<int>(type: "int", nullable: true),
                    price = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    sale_type = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true, defaultValueSql: "('FIXED')"),
                    image_url = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    status = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true, defaultValueSql: "('AVAILABLE')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artwork", x => x.artwork_id);
                    table.ForeignKey(
                        name: "FK__artwork__artist___44FF419A",
                        column: x => x.artist_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK__artwork__categor__45F365D3",
                        column: x => x.category_id,
                        principalTable: "Category",
                        principalColumn: "category_id");
                });

            migrationBuilder.CreateTable(
                name: "Gallery",
                columns: table => new
                {
                    gallery_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gallery", x => x.gallery_id);
                    table.ForeignKey(
                        name: "FK__gallery__user_id__49C3F6B7",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Auction",
                columns: table => new
                {
                    auction_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    artwork_id = table.Column<int>(type: "int", nullable: false),
                    start_price = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    current_price = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    start_time = table.Column<DateTime>(type: "datetime", nullable: false),
                    end_time = table.Column<DateTime>(type: "datetime", nullable: false),
                    status = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true, defaultValueSql: "('ONGOING')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auction", x => x.auction_id);
                    table.ForeignKey(
                        name: "FK__auction__artwork__534D60F1",
                        column: x => x.artwork_id,
                        principalTable: "Artwork",
                        principalColumn: "artwork_id");
                });

            migrationBuilder.CreateTable(
                name: "Exhibition_request",
                columns: table => new
                {
                    request_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    artwork_id = table.Column<int>(type: "int", nullable: false),
                    exhibition_id = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true, defaultValueSql: "('PENDING')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__exhibiti__18D3B90FEA1C0401", x => x.request_id);
                    table.ForeignKey(
                        name: "FK__exhibitio__artwo__72C60C4A",
                        column: x => x.artwork_id,
                        principalTable: "Artwork",
                        principalColumn: "artwork_id");
                    table.ForeignKey(
                        name: "FK__exhibitio__exhib__73BA3083",
                        column: x => x.exhibition_id,
                        principalTable: "Exhibition",
                        principalColumn: "exhibition_id");
                    table.ForeignKey(
                        name: "FK__exhibitio__user___71D1E811",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    order_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    artwork_id = table.Column<int>(type: "int", nullable: false),
                    total_amount = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    order_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    status = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true, defaultValueSql: "('PENDING')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.order_id);
                    table.ForeignKey(
                        name: "FK__orders__artwork___5EBF139D",
                        column: x => x.artwork_id,
                        principalTable: "Artwork",
                        principalColumn: "artwork_id");
                    table.ForeignKey(
                        name: "FK__orders__user_id__5DCAEF64",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Review",
                columns: table => new
                {
                    review_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    artwork_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    rating = table.Column<int>(type: "int", nullable: true),
                    comment = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Review", x => x.review_id);
                    table.ForeignKey(
                        name: "FK__review__artwork___6A30C649",
                        column: x => x.artwork_id,
                        principalTable: "Artwork",
                        principalColumn: "artwork_id");
                    table.ForeignKey(
                        name: "FK__review__user_id__6B24EA82",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Gallery_artwork",
                columns: table => new
                {
                    gallery_id = table.Column<int>(type: "int", nullable: false),
                    artwork_id = table.Column<int>(type: "int", nullable: false),
                    added_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__gallery___EFC61B7C0ACF6BBE", x => new { x.gallery_id, x.artwork_id });
                    table.ForeignKey(
                        name: "FK__gallery_a__artwo__4E88ABD4",
                        column: x => x.artwork_id,
                        principalTable: "Artwork",
                        principalColumn: "artwork_id");
                    table.ForeignKey(
                        name: "FK__gallery_a__galle__4D94879B",
                        column: x => x.gallery_id,
                        principalTable: "Gallery",
                        principalColumn: "gallery_id");
                });

            migrationBuilder.CreateTable(
                name: "Bid",
                columns: table => new
                {
                    bid_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    auction_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    amount = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    bid_time = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bid", x => x.bid_id);
                    table.ForeignKey(
                        name: "FK__bid__auction_id__571DF1D5",
                        column: x => x.auction_id,
                        principalTable: "Auction",
                        principalColumn: "auction_id");
                    table.ForeignKey(
                        name: "FK__bid__user_id__5812160E",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    payment_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    order_id = table.Column<int>(type: "int", nullable: false),
                    method = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    payment_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    amount = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    status = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true, defaultValueSql: "('PENDING')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.payment_id);
                    table.ForeignKey(
                        name: "FK__payment__order_i__656C112C",
                        column: x => x.order_id,
                        principalTable: "Orders",
                        principalColumn: "order_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Artwork_artist_id",
                table: "Artwork",
                column: "artist_id");

            migrationBuilder.CreateIndex(
                name: "IX_Artwork_category_id",
                table: "Artwork",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_Auction_artwork_id",
                table: "Auction",
                column: "artwork_id");

            migrationBuilder.CreateIndex(
                name: "IX_Bid_auction_id",
                table: "Bid",
                column: "auction_id");

            migrationBuilder.CreateIndex(
                name: "IX_Bid_user_id",
                table: "Bid",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Exhibition_request_artwork_id",
                table: "Exhibition_request",
                column: "artwork_id");

            migrationBuilder.CreateIndex(
                name: "IX_Exhibition_request_exhibition_id",
                table: "Exhibition_request",
                column: "exhibition_id");

            migrationBuilder.CreateIndex(
                name: "IX_Exhibition_request_user_id",
                table: "Exhibition_request",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Gallery_user_id",
                table: "Gallery",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Gallery_artwork_artwork_id",
                table: "Gallery_artwork",
                column: "artwork_id");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_artwork_id",
                table: "Orders",
                column: "artwork_id");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_user_id",
                table: "Orders",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_order_id",
                table: "Payment",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_Review_artwork_id",
                table: "Review",
                column: "artwork_id");

            migrationBuilder.CreateIndex(
                name: "IX_Review_user_id",
                table: "Review",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "UQ__users__AB6E61641EE8BD28",
                table: "Users",
                column: "email",
                unique: true,
                filter: "[email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UQ__users__F3DBC572D70F49A1",
                table: "Users",
                column: "username",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admin");

            migrationBuilder.DropTable(
                name: "Bid");

            migrationBuilder.DropTable(
                name: "Config");

            migrationBuilder.DropTable(
                name: "Contact");

            migrationBuilder.DropTable(
                name: "Exhibition_request");

            migrationBuilder.DropTable(
                name: "Gallery_artwork");

            migrationBuilder.DropTable(
                name: "Page");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "Review");

            migrationBuilder.DropTable(
                name: "Auction");

            migrationBuilder.DropTable(
                name: "Exhibition");

            migrationBuilder.DropTable(
                name: "Gallery");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Artwork");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Category");
        }
    }
}
