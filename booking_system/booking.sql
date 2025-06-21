-- -------------------------------------------------------------
-- TablePlus 6.4.8(608)
--
-- https://tableplus.com/
--
-- Database: booking
-- Generation Time: 2025-06-21 7:17:50.1830 pm
-- -------------------------------------------------------------


DROP TABLE IF EXISTS "public"."__EFMigrationsHistory";
-- Table Definition
CREATE TABLE "public"."__EFMigrationsHistory" (
    "MigrationId" varchar(150) NOT NULL,
    "ProductVersion" varchar(32) NOT NULL,
    PRIMARY KEY ("MigrationId")
);

DROP TABLE IF EXISTS "public"."Users";
-- Table Definition
CREATE TABLE "public"."Users" (
    "Id" uuid NOT NULL,
    "Phone" text,
    "Email" text NOT NULL DEFAULT ''::text,
    "CountryId" uuid NOT NULL,
    "DateOfBirth" date,
    "Address" text,
    "ProfileImage" text,
    "RegisterDate" timestamptz NOT NULL,
    "Status" int4 NOT NULL,
    "FirebaseUserId" text NOT NULL,
    "LoginType" text,
    "IsEmailVerified" bool NOT NULL,
    "CreatedAt" timestamptz NOT NULL,
    "UpdatedAt" timestamptz NOT NULL,
    "Name" text NOT NULL DEFAULT ''::text,
    "VerificationToken" uuid,
    PRIMARY KEY ("Id")
);

DROP TABLE IF EXISTS "public"."Packages";
-- Table Definition
CREATE TABLE "public"."Packages" (
    "Id" uuid NOT NULL,
    "Name" text NOT NULL,
    "Image" text,
    "Description" text,
    "GatewayId" uuid NOT NULL,
    "Price" float4 NOT NULL,
    "Currency" text NOT NULL,
    "PlanId" text NOT NULL,
    "TermsAndCondition" text,
    "IsActive" bool NOT NULL,
    "Credit" int4 NOT NULL,
    "ExpiredDuration" int4 NOT NULL,
    "CreatedAt" timestamptz NOT NULL,
    "UpdatedAt" timestamptz NOT NULL,
    PRIMARY KEY ("Id")
);

DROP TABLE IF EXISTS "public"."Countries";
-- Table Definition
CREATE TABLE "public"."Countries" (
    "Id" uuid NOT NULL,
    "Name" text NOT NULL,
    "CreatedAt" timestamptz NOT NULL,
    PRIMARY KEY ("Id")
);

DROP TABLE IF EXISTS "public"."Classes";
-- Table Definition
CREATE TABLE "public"."Classes" (
    "Id" uuid NOT NULL,
    "Name" text NOT NULL,
    "CountryId" uuid NOT NULL,
    "RequiredCredit" int4 NOT NULL,
    "StartTime" timestamptz NOT NULL,
    "EndTime" timestamptz NOT NULL,
    "MaxParticipants" int4 NOT NULL,
    "Status" int4 NOT NULL,
    "CreatedAt" timestamptz NOT NULL,
    "UpdatedAt" timestamptz NOT NULL,
    PRIMARY KEY ("Id")
);

DROP TABLE IF EXISTS "public"."PaymentGateways";
-- Table Definition
CREATE TABLE "public"."PaymentGateways" (
    "Id" uuid NOT NULL,
    "Platform" text NOT NULL,
    "DisplayName" text NOT NULL,
    "CountryId" uuid NOT NULL,
    "Image" text,
    "IsActive" bool NOT NULL,
    "CreatedAt" timestamptz NOT NULL,
    "UpdatedAt" timestamptz NOT NULL,
    PRIMARY KEY ("Id")
);

DROP TABLE IF EXISTS "public"."GatewayRawEvents";
-- Table Definition
CREATE TABLE "public"."GatewayRawEvents" (
    "Id" uuid NOT NULL,
    "GateWayOrderId" text NOT NULL,
    "EventType" text NOT NULL,
    "CallbackResponsePayload" json,
    "RequestPayload" json,
    "TranResponsePayload" json,
    "CreatedAt" timestamptz NOT NULL,
    "UpdatedAt" timestamptz NOT NULL,
    PRIMARY KEY ("Id")
);

DROP TABLE IF EXISTS "public"."ClassBookings";
-- Table Definition
CREATE TABLE "public"."ClassBookings" (
    "Id" uuid NOT NULL,
    "ClassId" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "UserCreditId" uuid NOT NULL,
    "Status" int4 NOT NULL,
    "BookAt" timestamptz NOT NULL,
    "CancelledAt" timestamptz NOT NULL,
    "IsRefund" bool NOT NULL,
    "CreatedAt" timestamptz NOT NULL,
    "UpdatedAt" timestamptz NOT NULL,
    PRIMARY KEY ("Id")
);

DROP TABLE IF EXISTS "public"."Refunds";
-- Table Definition
CREATE TABLE "public"."Refunds" (
    "Id" uuid NOT NULL,
    "ClassBookingId" uuid,
    "Type" int4 NOT NULL,
    "RefundAmount" float4 NOT NULL,
    "RefundCurrency" text,
    "RefundedAt" timestamptz NOT NULL,
    "Note" text,
    "CreatedAt" timestamptz NOT NULL,
    "UpdatedAt" timestamptz NOT NULL,
    PRIMARY KEY ("Id")
);

DROP TABLE IF EXISTS "public"."UserCreditHistories";
-- Table Definition
CREATE TABLE "public"."UserCreditHistories" (
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "TransactionId" uuid NOT NULL,
    "RefundId" uuid NOT NULL,
    "TotalCredit" int4 NOT NULL,
    "Type" int4 NOT NULL,
    "IsExpired" bool NOT NULL,
    "CreatedAt" timestamptz NOT NULL,
    "UpdatedAt" timestamptz NOT NULL,
    PRIMARY KEY ("Id")
);

DROP TABLE IF EXISTS "public"."Transactions";
-- Table Definition
CREATE TABLE "public"."Transactions" (
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "ExpiredAt" timestamptz,
    "PackageId" uuid NOT NULL,
    "Amount" float4 NOT NULL,
    "Currency" text,
    "Status" int4 NOT NULL,
    "GateWayOrderId" text,
    "GateRefCode" text,
    "Message" text,
    "GateWayRawEventId" uuid NOT NULL,
    "CreatedAt" timestamptz NOT NULL,
    "UpdatedAt" timestamptz NOT NULL,
    "AppliedPlan" text,
    "Platform" text,
    "RequestedPlan" text,
    PRIMARY KEY ("Id")
);

INSERT INTO "public"."__EFMigrationsHistory" ("MigrationId", "ProductVersion") VALUES
('20250621092926_Initial', '8.0.10'),
('20250621104714_Second', '8.0.10'),
('20250621113723_Third', '8.0.10'),
('20250621124704_payment', '8.0.10');

INSERT INTO "public"."Users" ("Id", "Phone", "Email", "CountryId", "DateOfBirth", "Address", "ProfileImage", "RegisterDate", "Status", "FirebaseUserId", "LoginType", "IsEmailVerified", "CreatedAt", "UpdatedAt", "Name", "VerificationToken") VALUES
('39d8a1cd-5607-4d74-a141-7226142c36c6', '09427371700', 'eimyatmon759@gmail.com', '9dde76cb-b052-4365-b528-ec2ecc13997b', NULL, 'Yangon', NULL, '2025-06-21 11:11:16.528064+00', 0, 'Ik7MATZdq7No9BExBi8VjlvIzeu1', 'password', 'f', '2025-06-21 11:11:16.526662+00', '2025-06-21 11:11:16.526662+00', 'Ei Myat Myat Mon', NULL);

INSERT INTO "public"."Packages" ("Id", "Name", "Image", "Description", "GatewayId", "Price", "Currency", "PlanId", "TermsAndCondition", "IsActive", "Credit", "ExpiredDuration", "CreatedAt", "UpdatedAt") VALUES
('c6f75a39-342a-4c59-a591-9e69ef20064b', 'Basic Package', 'Image', 'Description', '4a51f122-030c-4245-ad15-ce700a3a96be', 10, 'Dollar', 'basic', 'Basic Package for SG', 't', 5, 10, '2025-06-21 07:07:25.432581+00', '2025-06-21 07:07:25.432581+00');

INSERT INTO "public"."Countries" ("Id", "Name", "CreatedAt") VALUES
('9dde76cb-b052-4365-b528-ec2ecc13997b', 'Myanmar', '2025-06-21 07:07:25.432581+00');

INSERT INTO "public"."PaymentGateways" ("Id", "Platform", "DisplayName", "CountryId", "Image", "IsActive", "CreatedAt", "UpdatedAt") VALUES
('4a51f122-030c-4245-ad15-ce700a3a96be', 'SG payment', 'SG Payment', '9dde76cb-b052-4365-b528-ec2ecc13997b', NULL, 't', '2025-06-21 07:07:25.432581+00', '2025-06-21 07:07:25.432581+00');



-- Indices
CREATE UNIQUE INDEX "PK___EFMigrationsHistory" ON public."__EFMigrationsHistory" USING btree ("MigrationId");
ALTER TABLE "public"."Users" ADD FOREIGN KEY ("CountryId") REFERENCES "public"."Countries"("Id") ON DELETE CASCADE;


-- Indices
CREATE UNIQUE INDEX "PK_Users" ON public."Users" USING btree ("Id");
CREATE INDEX "IX_Users_CountryId" ON public."Users" USING btree ("CountryId");
ALTER TABLE "public"."Packages" ADD FOREIGN KEY ("GatewayId") REFERENCES "public"."PaymentGateways"("Id") ON DELETE CASCADE;


-- Indices
CREATE UNIQUE INDEX "PK_Packages" ON public."Packages" USING btree ("Id");
CREATE INDEX "IX_Packages_GatewayId" ON public."Packages" USING btree ("GatewayId");


-- Indices
CREATE UNIQUE INDEX "PK_Countries" ON public."Countries" USING btree ("Id");
ALTER TABLE "public"."Classes" ADD FOREIGN KEY ("CountryId") REFERENCES "public"."Countries"("Id") ON DELETE CASCADE;


-- Indices
CREATE UNIQUE INDEX "PK_Classes" ON public."Classes" USING btree ("Id");
CREATE INDEX "IX_Classes_CountryId" ON public."Classes" USING btree ("CountryId");
ALTER TABLE "public"."PaymentGateways" ADD FOREIGN KEY ("CountryId") REFERENCES "public"."Countries"("Id") ON DELETE CASCADE;


-- Indices
CREATE UNIQUE INDEX "PK_PaymentGateways" ON public."PaymentGateways" USING btree ("Id");
CREATE INDEX "IX_PaymentGateways_CountryId" ON public."PaymentGateways" USING btree ("CountryId");


-- Indices
CREATE UNIQUE INDEX "PK_GatewayRawEvents" ON public."GatewayRawEvents" USING btree ("Id");
ALTER TABLE "public"."ClassBookings" ADD FOREIGN KEY ("UserId") REFERENCES "public"."Users"("Id") ON DELETE CASCADE;
ALTER TABLE "public"."ClassBookings" ADD FOREIGN KEY ("ClassId") REFERENCES "public"."Classes"("Id") ON DELETE CASCADE;
ALTER TABLE "public"."ClassBookings" ADD FOREIGN KEY ("UserCreditId") REFERENCES "public"."UserCreditHistories"("Id") ON DELETE CASCADE;


-- Indices
CREATE UNIQUE INDEX "PK_ClassBookings" ON public."ClassBookings" USING btree ("Id");
CREATE INDEX "IX_ClassBookings_ClassId" ON public."ClassBookings" USING btree ("ClassId");
CREATE INDEX "IX_ClassBookings_UserCreditId" ON public."ClassBookings" USING btree ("UserCreditId");
CREATE INDEX "IX_ClassBookings_UserId" ON public."ClassBookings" USING btree ("UserId");
ALTER TABLE "public"."Refunds" ADD FOREIGN KEY ("ClassBookingId") REFERENCES "public"."ClassBookings"("Id") ON DELETE CASCADE;


-- Indices
CREATE UNIQUE INDEX "PK_Refunds" ON public."Refunds" USING btree ("Id");
CREATE UNIQUE INDEX "IX_Refunds_ClassBookingId" ON public."Refunds" USING btree ("ClassBookingId");
ALTER TABLE "public"."UserCreditHistories" ADD FOREIGN KEY ("UserId") REFERENCES "public"."Users"("Id") ON DELETE CASCADE;
ALTER TABLE "public"."UserCreditHistories" ADD FOREIGN KEY ("TransactionId") REFERENCES "public"."Transactions"("Id") ON DELETE CASCADE;
ALTER TABLE "public"."UserCreditHistories" ADD FOREIGN KEY ("RefundId") REFERENCES "public"."Refunds"("Id") ON DELETE CASCADE;


-- Indices
CREATE UNIQUE INDEX "PK_UserCreditHistories" ON public."UserCreditHistories" USING btree ("Id");
CREATE UNIQUE INDEX "IX_UserCreditHistories_RefundId" ON public."UserCreditHistories" USING btree ("RefundId");
CREATE UNIQUE INDEX "IX_UserCreditHistories_TransactionId" ON public."UserCreditHistories" USING btree ("TransactionId");
CREATE INDEX "IX_UserCreditHistories_UserId" ON public."UserCreditHistories" USING btree ("UserId");
ALTER TABLE "public"."Transactions" ADD FOREIGN KEY ("PackageId") REFERENCES "public"."Packages"("Id") ON DELETE CASCADE;
ALTER TABLE "public"."Transactions" ADD FOREIGN KEY ("GateWayRawEventId") REFERENCES "public"."GatewayRawEvents"("Id") ON DELETE CASCADE;
ALTER TABLE "public"."Transactions" ADD FOREIGN KEY ("UserId") REFERENCES "public"."Users"("Id") ON DELETE CASCADE;


-- Indices
CREATE UNIQUE INDEX "PK_Transactions" ON public."Transactions" USING btree ("Id");
CREATE UNIQUE INDEX "IX_Transactions_GateWayRawEventId" ON public."Transactions" USING btree ("GateWayRawEventId");
CREATE INDEX "IX_Transactions_PackageId" ON public."Transactions" USING btree ("PackageId");
CREATE INDEX "IX_Transactions_UserId" ON public."Transactions" USING btree ("UserId");
