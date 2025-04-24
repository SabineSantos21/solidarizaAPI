-- Criação do banco de dados
CREATE DATABASE IF NOT EXISTS solidarizadb;
USE solidarizadb;

-- Tabela User
CREATE TABLE User (
    userId INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    type INT NOT NULL,
    documentType INT,
    documentNumber VARCHAR(50),
    email VARCHAR(255) NOT NULL,
    phone VARCHAR(50),
    password VARCHAR(255) NOT NULL,
    creationDate DATE,
    isActive BOOLEAN DEFAULT TRUE
);

-- Tabela Profile
CREATE TABLE Profile (
    profileId INT AUTO_INCREMENT PRIMARY KEY,
    userId INT,
    name VARCHAR(255) NOT NULL,
    description VARCHAR(512),
    address VARCHAR(255),
    city VARCHAR(255),
    state VARCHAR(255),
    zip VARCHAR(255),
    FOREIGN KEY (userId) REFERENCES User(userId) ON DELETE CASCADE
);

-- Tabela Link
CREATE TABLE Link (
    linkId INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    url VARCHAR(512) NOT NULL,
    type INT,
    profileId INT,
    FOREIGN KEY (profileId) REFERENCES Profile(profileId) ON DELETE CASCADE
);

-- Tabela Campaign
CREATE TABLE Campaign (
    campaignId INT AUTO_INCREMENT PRIMARY KEY,
    userId INT,
    type INT NOT NULL,
    title VARCHAR(255) NOT NULL,
    description VARCHAR(512),
    startDate DATETIME,
    endDate DATETIME,
	address VARCHAR(255),
    city VARCHAR(255),
    state VARCHAR(255),
    status INT NOT NULL,
    FOREIGN KEY (userId) REFERENCES User(userId) ON DELETE SET NULL
);

-- Tabela Campaign_Volunteers
CREATE TABLE Campaign_Volunteers (
	campaignVolunteerId INT AUTO_INCREMENT PRIMARY KEY,
    campaignId INT,
    userId INT,
    isApproved INT,
    FOREIGN KEY (campaignId) REFERENCES Campaign(campaignId) ON DELETE CASCADE,
    FOREIGN KEY (userId) REFERENCES User(userId) ON DELETE CASCADE
);

-- Tabela Organization_Info
CREATE TABLE Organization_Info (
	organizationInfoId INT AUTO_INCREMENT PRIMARY KEY,
    userId INT,
    isOrganizationApproved BOOLEAN DEFAULT FALSE,
    disapprovalReason VARCHAR(512),
    pixType INT,
    pixKey VARCHAR(50),
    beneficiaryName VARCHAR(255),
    beneficiaryCity VARCHAR(255),
    pixValue VARCHAR(255),
    contactName VARCHAR(255),
    contactPhone VARCHAR(50),
    FOREIGN KEY (userId) REFERENCES User(userId) ON DELETE CASCADE
);