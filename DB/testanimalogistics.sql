-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 01-06-2024 a las 22:23:08
-- Versión del servidor: 10.4.32-MariaDB
-- Versión de PHP: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `testanimalogistics`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `animales`
--

CREATE TABLE `animales` (
  `Id` int(11) NOT NULL,
  `RefugioId` int(11) DEFAULT NULL,
  `UsuarioId` int(11) NOT NULL,
  `Nombre` varchar(255) DEFAULT NULL,
  `Edad` varchar(20) DEFAULT NULL,
  `Tipo` varchar(255) DEFAULT NULL,
  `Tamano` varchar(20) DEFAULT NULL,
  `Collar` tinyint(1) DEFAULT NULL,
  `Genero` varchar(20) DEFAULT NULL,
  `Comentarios` varchar(255) DEFAULT NULL,
  `FotoUrl` varchar(255) DEFAULT NULL,
  `GPSY` decimal(8,6) DEFAULT NULL,
  `GPSX` decimal(9,6) DEFAULT NULL,
  `Estado` enum('Reportado','Buscando','En recuperacion','En adopcion') DEFAULT 'Reportado'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `animales`
--

INSERT INTO `animales` (`Id`, `RefugioId`, `UsuarioId`, `Nombre`, `Edad`, `Tipo`, `Tamano`, `Collar`, `Genero`, `Comentarios`, `FotoUrl`, `GPSY`, `GPSX`, `Estado`) VALUES
(10, 1, 3, NULL, '2', 'Perro', 'Mediano', 1, 'Macho', 'Cariñoso y juguetón', 'https://picsum.photos/200\n', 40.712800, -74.006000, 'En adopcion'),
(11, 16, 3, 'Luna', '1', 'Gato', 'Pequeño', 0, 'Hembra', 'Rescatada de la calle', 'https://picsum.photos/200\n', 34.052200, -100.000000, 'En adopcion'),
(51, NULL, 8, 'SOYNOMBREunoe', '6', 'SOYTIPO', 'Mediano', 1, 'Hembra', 'ES una Nutria salvaje', 'https://picsum.photos/200', -56.337447, -33.268252, 'En adopcion'),
(52, 16, 8, 'SOYNOMBREdosedittectttttt', '4', 'SOYTIPO', 'Grande', 1, 'Macho', 'ES una Nutria salvaje', 'Data/usuario/animal/cccb15cf-d0f4-4bae-8420-c86941c8e7ee.jpg', -66.337447, -33.268252, 'En recuperacion'),
(54, NULL, 8, 'SOYNOMBRE', '0.0', 'SOYTIPO', 'Grande', 1, NULL, 'ES una Nutria salvaje', NULL, -66.337473, -33.268249, 'Reportado'),
(55, NULL, 8, 'SOYNOMBRE', '0.0', 'SOYTIPO', 'Pequeño', 1, NULL, 'ES una Nutria salvaje', NULL, -66.337473, -33.268249, 'Reportado'),
(56, NULL, 8, 'SOYNOMBRE', '0.0', 'SOYTIPO', 'Mediano', 1, NULL, 'ES una Nutria salvaje', NULL, -66.337473, -33.268249, 'Reportado'),
(57, NULL, 8, 'SOYNOMBRE', '4.0', 'SOYTIPO', 'Pequeño', 1, 'Macho', 'ES una Nutria salvaje', NULL, -66.337417, -33.268213, 'Reportado'),
(58, NULL, 8, 'SOYNOMBRE', '3.0', 'SOYTIPO', 'Mediano', 1, 'Hembra', 'ES una Nutria salvaje', NULL, -66.297193, -33.215015, 'Reportado');

--
-- Disparadores `animales`
--
DELIMITER $$
CREATE TRIGGER `estado_default_animal` BEFORE INSERT ON `animales` FOR EACH ROW BEGIN
    IF NEW.Estado IS NULL THEN
        SET NEW.Estado = 'Reportado';
    END IF;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `eventos`
--

CREATE TABLE `eventos` (
  `Id` int(11) NOT NULL,
  `RefugioId` int(11) NOT NULL,
  `FechaDesde` datetime DEFAULT NULL,
  `FechaHasta` datetime DEFAULT NULL,
  `Descripcion` varchar(255) DEFAULT NULL,
  `Estado` enum('Programado','En curso','Finalizado','Cancelado') NOT NULL DEFAULT 'Programado'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `eventos`
--

INSERT INTO `eventos` (`Id`, `RefugioId`, `FechaDesde`, `FechaHasta`, `Descripcion`, `Estado`) VALUES
(1, 1, '2024-04-10 10:00:00', '2024-04-10 14:00:00', 'Jornada de adopción de mascotas', 'Programado'),
(2, 1, '2024-04-15 09:00:00', '2024-04-15 17:00:00', 'Charla sobre cuidados de animales domésticos', 'Programado');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `noticias`
--

CREATE TABLE `noticias` (
  `Id` int(11) NOT NULL,
  `VoluntarioId` int(11) NOT NULL,
  `BannerUrl` varchar(255) DEFAULT NULL,
  `Categoria` varchar(255) DEFAULT NULL,
  `Titulo` varchar(255) DEFAULT NULL,
  `Contenido` varchar(1000) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `noticias`
--

INSERT INTO `noticias` (`Id`, `VoluntarioId`, `BannerUrl`, `Categoria`, `Titulo`, `Contenido`) VALUES
(3, 16, 'https://random.imagecdn.app/200/200', 'Adopciones', 'Nueva camada de cachorros en busca de hogar', 'Visítanos y conoce a los nuevos integrantes de nuestra familia.'),
(4, 1, 'https://random.imagecdn.app/200/200', 'Eventos', 'Próxima jornada de adopción de animales domésticos', '¡Estamos emocionados de anunciar nuestra próxima jornada de adopción de animales domésticos!\n\nEl [nombre del refugio] se complace en invitarlos a nuestra próxima jornada de adopción de animales domésticos que se llevará a cabo el próximo fin de semana. Esta es una oportunidad perfecta para encontrar a tu nuevo compañero peludo y darle un hogar amoroso para siempre.\n\nDurante este evento, tendrás la oportunidad de conocer a una variedad de perros y gatos que están buscando su hogar permanente. Nuestro personal estará disponible para ayudarte a encontrar el animal que mejor se adapte a tu estilo de vida y necesidades.\n\nNo te pierdas esta maravillosa oportunidad de encontrar a tu nuevo mejor amigo. ¡Esperamos verte el próximo fin de semana en nuestra jornada de adopción de animales domésticos!\n\n¡No olvides difundir la palabra y ayudarnos a encontrar hogares amorosos para todos nuestros amigos peludos!\n\n¡Te esperamos!'),
(10, 5, 'url_imagen_1.jpg', 'Refugio de Animales', 'Nueva adopción en el refugio', 'Hoy hemos logrado encontrar un nuevo hogar para uno de nuestros animales. ¡Estamos muy felices por él!'),
(11, 1, 'url_imagen_2.jpg', 'Refugio de Animales', 'Jornada de adopción este fin de semana', 'Te esperamos este sábado y domingo en nuestro refugio para conocer a nuestros animales en adopción.'),
(12, 5, 'url_imagen_3.jpg', 'Refugio de Animales', 'Campaña de esterilización gratuita', 'Este mes estaremos realizando esterilizaciones gratuitas para controlar la población de animales callejeros.'),
(13, 1, 'url_imagen_4.jpg', 'Refugio de Animales', 'Necesitamos voluntarios para pasear a nuestros perros', 'Buscamos personas amantes de los animales que nos ayuden a pasear a nuestros perros cada tarde.'),
(14, 6, 'url_imagen_5.jpg', 'Refugio de Animales', 'Donación de alimentos para nuestros gatos', '¡Gracias a la generosidad de nuestros colaboradores, nuestros gatos tienen comida para todo el mes!');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `refugios`
--

CREATE TABLE `refugios` (
  `Id` int(11) NOT NULL,
  `UsuarioId` int(11) NOT NULL,
  `Nombre` varchar(255) DEFAULT NULL,
  `Direccion` varchar(255) DEFAULT NULL,
  `Descripcion` varchar(255) DEFAULT NULL,
  `Telefono` varchar(20) DEFAULT NULL,
  `GPSY` decimal(8,6) DEFAULT NULL,
  `GPSX` decimal(9,6) DEFAULT NULL,
  `GPSRango` int(11) DEFAULT NULL,
  `BannerUrl` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `refugios`
--

INSERT INTO `refugios` (`Id`, `UsuarioId`, `Nombre`, `Direccion`, `Descripcion`, `Telefono`, `GPSY`, `GPSX`, `GPSRango`, `BannerUrl`) VALUES
(1, 2, 'Amigos de los Animales', 'Avenida Central 456', 'Asociación sin fines de lucro para la protección animal', '987-654-3210', -66.268180, -33.337465, 1000, 'https://random.imagecdn.app/200/200'),
(6, 2, 'Save The Dodo Asociation', 'Parque de los animales 730', 'Dedicados a hacer historia', '2664777777', -66.288012, -33.339856, 500, 'https://random.imagecdn.app/201/200'),
(12, 8, 'Refugio Esperanza', 'Calle Falsa 123', 'Refugio para animales abandonados', '555-1234', -67.603722, -25.381592, 100, 'http://example.com/banner1.jpg'),
(13, 8, 'Hogar de Animales', 'Avenida Siempre Viva 742', 'Protección y cuidado de animales', '555-5678', -67.615803, -25.433298, 200, 'http://example.com/banner2.jpg'),
(14, 8, 'Casa de Mascotas', 'Calle Luna 456', 'Refugio especializado en gatos', '555-8765', -68.599722, -30.381111, 150, 'http://example.com/banner3.jpg'),
(15, 8, 'Amigos Peludos', 'Calle Estrella 789', 'Rescate y adopción de perros', '555-4321', -69.607222, -31.383611, 120, 'http://example.com/banner4.jpg'),
(16, 8, 'Refugio Patitas', 'Avenida Sol 1011', 'Cuidado integral para animales domésticos', '555-9999', -28.614722, -35.385000, 180, 'http://example.com/banner5.jpg');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `tareas`
--

CREATE TABLE `tareas` (
  `Id` int(11) NOT NULL,
  `Actividad` varchar(150) NOT NULL,
  `Descripcion` varchar(250) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `tareas`
--

INSERT INTO `tareas` (`Id`, `Actividad`, `Descripcion`) VALUES
(1, 'Pasear perros', 'Sacar a pasear a los perros del refugio por las mañanas.'),
(2, 'Limpieza de instalaciones', 'Limpiar las jaulas y áreas de los animales.'),
(3, 'Alimentación', 'Preparar y distribuir la comida para los animales.'),
(4, 'Cuidado veterinario', 'Asistir al veterinario en la atención de animales enfermos.'),
(5, 'Socialización de gatos', 'Jugar y socializar con los gatos del refugio.'),
(6, 'Mantenimiento de jardines', 'Cuidar y mantener las áreas verdes del refugio.'),
(7, 'Recepción de animales', 'Recibir y registrar a los animales que ingresan al refugio.'),
(8, 'Campañas de adopción', 'Organizar y participar en campañas de adopción de animales.'),
(9, 'Educación comunitaria', 'Impartir charlas sobre tenencia responsable de mascotas.'),
(10, 'Administración', 'Ayudar con tareas administrativas del refugio.');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usuarios`
--

CREATE TABLE `usuarios` (
  `Id` int(11) NOT NULL,
  `Nombre` varchar(255) DEFAULT NULL,
  `Apellido` varchar(255) DEFAULT NULL,
  `DNI` varchar(20) DEFAULT NULL,
  `Telefono` varchar(250) NOT NULL,
  `Correo` varchar(250) NOT NULL,
  `Contrasena` varchar(255) DEFAULT NULL,
  `FotoUrl` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `usuarios`
--

INSERT INTO `usuarios` (`Id`, `Nombre`, `Apellido`, `DNI`, `Telefono`, `Correo`, `Contrasena`, `FotoUrl`) VALUES
(1, 'Juan', 'Gómez', '12345678', '26647878', 'Gomez@hotmail.com', 'contraseña123', 'http://ejemplo.com/juan.jpg'),
(2, 'María', 'López', '87654321', '36524444', 'Lopez@hotmail.com', 'clave456', 'http://ejemplo.com/maria.jpg'),
(3, 'Albert', 'Tesla', '38439123', '08001010', 'Tesla@hotmail.com', 'contraseña123', 'http://ejemplo.com/Albert.jpg'),
(8, 'Marti', 'Panelo', '38383838', '2664878787', 'martin@panelo.com', 'ipKy7T47aO1Abua2D4f7lyBoJaTBcnE1jdSE0iGhDLI=', 'Data/usuario/avatar_825d8925c-3670-4f12-b013-e9c34e1ae660.jpg'),
(11, 'NombreTest', 'ApellidoTest', '43534539', '266494949', 'test@hotmail.com', 'ipKy7T47aO1Abua2D4f7lyBoJaTBcnE1jdSE0iGhDLI=', 'Data/usuario/avatar_11.png'),
(20, 'contra', 'contra', '11111111', '222222', 'contra@contra.com', 'ipKy7T47aO1Abua2D4f7lyBoJaTBcnE1jdSE0iGhDLI=', 'Data/usuario/avatar_20f6a3330d-2ec7-4cd0-8cd2-ae938fb1c6f7.jpg');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `voluntarios`
--

CREATE TABLE `voluntarios` (
  `Id` int(11) NOT NULL,
  `UsuarioId` int(11) DEFAULT NULL,
  `RefugioId` int(11) NOT NULL,
  `TareaId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `voluntarios`
--

INSERT INTO `voluntarios` (`Id`, `UsuarioId`, `RefugioId`, `TareaId`) VALUES
(1, 8, 6, 1),
(5, 3, 1, 2),
(6, 8, 15, 3),
(13, NULL, 15, 3),
(14, NULL, 6, 4),
(15, NULL, 16, 4),
(16, 8, 16, 4),
(20, NULL, 16, 5);

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `animales`
--
ALTER TABLE `animales`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `UsuarioId` (`UsuarioId`),
  ADD KEY `RefugioId` (`RefugioId`) USING BTREE;

--
-- Indices de la tabla `eventos`
--
ALTER TABLE `eventos`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `RefugioId` (`RefugioId`) USING BTREE;

--
-- Indices de la tabla `noticias`
--
ALTER TABLE `noticias`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `VoluntarioId` (`VoluntarioId`) USING BTREE;

--
-- Indices de la tabla `refugios`
--
ALTER TABLE `refugios`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `UsuarioId` (`UsuarioId`);

--
-- Indices de la tabla `tareas`
--
ALTER TABLE `tareas`
  ADD PRIMARY KEY (`Id`);

--
-- Indices de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  ADD PRIMARY KEY (`Id`);

--
-- Indices de la tabla `voluntarios`
--
ALTER TABLE `voluntarios`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `UsuarioId` (`UsuarioId`) USING BTREE,
  ADD KEY `RefugioId` (`RefugioId`) USING BTREE,
  ADD KEY `TareaId` (`TareaId`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `animales`
--
ALTER TABLE `animales`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=59;

--
-- AUTO_INCREMENT de la tabla `eventos`
--
ALTER TABLE `eventos`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT de la tabla `noticias`
--
ALTER TABLE `noticias`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=15;

--
-- AUTO_INCREMENT de la tabla `refugios`
--
ALTER TABLE `refugios`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=27;

--
-- AUTO_INCREMENT de la tabla `tareas`
--
ALTER TABLE `tareas`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=21;

--
-- AUTO_INCREMENT de la tabla `voluntarios`
--
ALTER TABLE `voluntarios`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=22;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `animales`
--
ALTER TABLE `animales`
  ADD CONSTRAINT `animal_ibfk_1` FOREIGN KEY (`RefugioId`) REFERENCES `refugios` (`Id`) ON DELETE CASCADE,
  ADD CONSTRAINT `animal_ibfk_2` FOREIGN KEY (`UsuarioId`) REFERENCES `usuarios` (`Id`) ON DELETE CASCADE;

--
-- Filtros para la tabla `eventos`
--
ALTER TABLE `eventos`
  ADD CONSTRAINT `evento_ibfk_1` FOREIGN KEY (`RefugioId`) REFERENCES `refugios` (`Id`) ON DELETE CASCADE;

--
-- Filtros para la tabla `noticias`
--
ALTER TABLE `noticias`
  ADD CONSTRAINT `noticias_ibfk_1` FOREIGN KEY (`VoluntarioId`) REFERENCES `voluntarios` (`Id`) ON DELETE CASCADE;

--
-- Filtros para la tabla `refugios`
--
ALTER TABLE `refugios`
  ADD CONSTRAINT `refugio_ibfk_1` FOREIGN KEY (`UsuarioId`) REFERENCES `usuarios` (`Id`) ON DELETE CASCADE;

--
-- Filtros para la tabla `voluntarios`
--
ALTER TABLE `voluntarios`
  ADD CONSTRAINT `voluntario_ibfk_1` FOREIGN KEY (`UsuarioId`) REFERENCES `usuarios` (`Id`) ON DELETE CASCADE,
  ADD CONSTRAINT `voluntario_ibfk_2` FOREIGN KEY (`RefugioId`) REFERENCES `refugios` (`Id`) ON DELETE CASCADE,
  ADD CONSTRAINT `voluntarios_ibfk_1` FOREIGN KEY (`TareaId`) REFERENCES `tareas` (`Id`) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
