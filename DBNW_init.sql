-- MySQL Script generated by MySQL Workbench
-- Tue Nov 21 15:47:50 2023
-- Model: New Model    Version: 1.0
-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- -----------------------------------------------------
-- Schema dbnw
-- -----------------------------------------------------

-- -----------------------------------------------------
-- Schema dbnw
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `dbnw` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci ;
USE `dbnw` ;

-- -----------------------------------------------------
-- Table `dbnw`.`login`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `dbnw`.`login` (
  `LoginID` VARCHAR(255) NOT NULL,
  PRIMARY KEY (`LoginID`),
  UNIQUE INDEX `LoginID_UNIQUE` (`LoginID` ASC) VISIBLE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `dbnw`.`userinfo`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `dbnw`.`userinfo` (
  `UserID` VARCHAR(255) NOT NULL,
  `Surname` VARCHAR(255) NULL DEFAULT NULL,
  `Fornames` VARCHAR(255) NOT NULL,
  `Title` VARCHAR(255) NOT NULL,
  `Position` VARCHAR(255) NOT NULL,
  `Phone` VARCHAR(13) NULL DEFAULT NULL,
  `Email` VARCHAR(255) NOT NULL,
  `Location` VARCHAR(255) NOT NULL,
  PRIMARY KEY (`UserID`),
  UNIQUE INDEX `UserID_UNIQUE` (`UserID` ASC) VISIBLE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `dbnw`.`user-login-join`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `dbnw`.`user-login-join` (
  `UserID` VARCHAR(255) NOT NULL,
  `LoginID` VARCHAR(255) NOT NULL,
  PRIMARY KEY (`LoginID`, `UserID`),
  INDEX `fk_UserInfo_has_Login_Login1_idx` (`LoginID` ASC) VISIBLE,
  INDEX `userfor_idx` (`UserID` ASC) VISIBLE,
  CONSTRAINT `loginfor`
    FOREIGN KEY (`LoginID`)
    REFERENCES `dbnw`.`login` (`LoginID`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `userfor`
    FOREIGN KEY (`UserID`)
    REFERENCES `dbnw`.`userinfo` (`UserID`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
