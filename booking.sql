INSERT INTO "public"."Refunds" ("Id", "BookingId", "Type", "RefundAmount", "RefundCurrency", "RefundedAt", "Note", "CreatedAt", "UpdatedAt") VALUES
('477d84a1-824c-42be-9a14-24cb7603d90c', 'dd742f1a-27f0-4597-a092-d4b602d7b22e', 0, 1, NULL, '2025-06-21 18:31:12.34417+00', 'Refund due to cancellation 4 hours before class start.', '2025-06-21 18:31:12.340144+00', '2025-06-21 18:31:12.340144+00');

INSERT INTO "public"."UserCreditHistories" ("Id", "UserId", "RefundId", "CreditAmount", "Type", "IsExpired", "CreatedAt", "UpdatedAt") VALUES
('92c2d2d5-dc20-4ade-a3de-7c9122177fa8', '5ead963d-9b3a-43e5-aac5-0f19a72e9469', NULL, 5, 0, 'f', '2025-06-21 17:37:41.852273+00', '2025-06-21 17:37:41.852273+00'),
('cfae0719-5013-4d87-a385-db63597f985d', '5ead963d-9b3a-43e5-aac5-0f19a72e9469', '477d84a1-824c-42be-9a14-24cb7603d90c', 1, 2, 'f', '2025-06-21 18:31:14.669258+00', '2025-06-21 18:31:14.665292+00'),
('d4ffa9ca-4456-45d3-9308-b4c4662fbd56', '5ead963d-9b3a-43e5-aac5-0f19a72e9469', NULL, 1, 1, 'f', '2025-06-21 18:01:50.442703+00', '2025-06-21 18:01:50.442704+00');

INSERT INTO "public"."Countries" ("Id", "Name", "CreatedAt") VALUES
('9dde76cb-b052-4365-b528-ec2ecc13997b', 'Myanmar', '2025-06-21 07:07:25.432581+00'),
('abcd1234-5678-90ab-cdef-1234567890ab', 'Thailand', '2025-06-22 09:00:00+00');

INSERT INTO "public"."__EFMigrationsHistory" ("MigrationId", "ProductVersion") VALUES
('20250621173423_booking', '8.0.10'),
('20250621175457_nullable', '8.0.10'),
('20250621181158_bookingId', '8.0.10'),
('20250621182214_navigationNull', '8.0.10');

INSERT INTO "public"."Classes" ("Id", "Name", "CountryId", "RequiredCredit", "StartTime", "EndTime", "MaxParticipants", "Status", "IsFull", "CreatedAt", "UpdatedAt") VALUES
('b6b6a999-4897-439b-a1b1-be770cb5ce6b', 'Yoga', '9dde76cb-b052-4365-b528-ec2ecc13997b', 1, '2025-06-21 17:57:26.739638+00', '2025-06-26 12:27:01+00', 10, 0, 'f', '2025-06-21 07:07:25.432581+00', '2025-06-21 07:07:25.432581+00'),
('c1d2e3f4-5678-90ab-cdef-abcdef123456', 'Pilates', 'abcd1234-5678-90ab-cdef-1234567890ab', 2, '2025-06-23 14:00:00+00', '2025-06-23 15:00:00+00', 15, 0, 'f', '2025-06-22 09:30:00+00', '2025-06-22 09:30:00+00');

INSERT INTO "public"."PaymentGateways" ("Id", "Platform", "DisplayName", "CountryId", "Image", "IsActive", "CreatedAt", "UpdatedAt") VALUES
('4a51f122-030c-4245-ad15-ce700a3a96be', 'SG payment', 'SG Payment', '9dde76cb-b052-4365-b528-ec2ecc13997b', NULL, 't', '2025-06-21 07:07:25.432581+00', '2025-06-21 07:07:25.432581+00'),
('d1e2f3a4-b5c6-47d8-9123-abcdefabcdef', 'Thailand Pay', 'Thailand Pay Gateway', 'abcd1234-5678-90ab-cdef-1234567890ab', NULL, 't', '2025-06-22 09:15:00+00', '2025-06-22 09:15:00+00');

INSERT INTO "public"."Users" ("Id", "Name", "CreditBalance", "Phone", "Email", "CountryId", "DateOfBirth", "Address", "ProfileImage", "RegisterDate", "Status", "FirebaseUserId", "LoginType", "IsEmailVerified", "VerificationToken", "CreatedAt", "UpdatedAt") VALUES
('5ead963d-9b3a-43e5-aac5-0f19a72e9469', 'Ei Myat Myat Mon', 5, '09427371700', 'eimyatmon759@gmail.com', '9dde76cb-b052-4365-b528-ec2ecc13997b', NULL, 'Yangon', NULL, '2025-06-21 17:37:41.852849+00', 0, 'E72YFpbfGeUXY2oLaRtI9A9ceYw2', 'password', 'f', NULL, '2025-06-21 17:37:41.852273+00', '2025-06-21 17:37:41.852273+00'),
('f1e2d3c4-b5a6-47b8-9123-abcdef987654', 'John Doe', 3, '0812345678', 'john.doe@example.com', 'abcd1234-5678-90ab-cdef-1234567890ab', NULL, 'Bangkok', NULL, '2025-06-22 09:10:00+00', 0, 'ABC123DEF456GHI789', 'password', 'f', NULL, '2025-06-22 09:10:00+00', '2025-06-22 09:10:00+00');

INSERT INTO "public"."Packages" ("Id", "Name", "Image", "Description", "GatewayId", "Price", "Currency", "PlanId", "TermsAndCondition", "IsActive", "Credit", "ExpiredDuration", "CreatedAt", "UpdatedAt") VALUES
('c6f75a39-342a-4c59-a591-9e69ef20064b', 'Basic Package', 'Image', 'Description', '4a51f122-030c-4245-ad15-ce700a3a96be', 10, 'Dollar', 'basic', 'Basic Package for SG', 't', 5, 10, '2025-06-21 07:07:25.432581+00', '2025-06-21 07:07:25.432581+00'),
('e1f2a3b4-c5d6-47e8-9123-abcdef654321', 'Premium Package', 'Image', 'Premium plan with extra credits', 'd1e2f3a4-b5c6-47d8-9123-abcdefabcdef', 20, 'Dollar', 'premium', 'Premium package terms', 't', 10, 30, '2025-06-22 09:20:00+00', '2025-06-22 09:20:00+00');

INSERT INTO "public"."ClassBookings" ("Id", "ClassId", "UserId", "UserCreditId", "Status", "BookAt", "CancelledAt", "CreatedAt", "UpdatedAt") VALUES
('dd742f1a-27f0-4597-a092-d4b602d7b22e', 'b6b6a999-4897-439b-a1b1-be770cb5ce6b', '5ead963d-9b3a-43e5-aac5-0f19a72e9469', 'd4ffa9ca-4456-45d3-9308-b4c4662fbd56', 2, '2025-06-21 18:01:51.397025+00', '2025-06-21 12:57:26.739638+00', '2025-06-21 18:01:51.394466+00', '2025-06-21 18:01:51.394466+00');
