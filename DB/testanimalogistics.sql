-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: localhost
-- Tiempo de generación: 24-06-2024 a las 06:00:18
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
  `Estado` enum('Reportado','En recuperacion','En adopcion') DEFAULT 'Reportado'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `animales`
--

INSERT INTO `animales` (`Id`, `RefugioId`, `UsuarioId`, `Nombre`, `Edad`, `Tipo`, `Tamano`, `Collar`, `Genero`, `Comentarios`, `FotoUrl`, `GPSY`, `GPSX`, `Estado`) VALUES
(73, 30, 1, 'Firulais', '3', 'Perro', 'Mediano', 1, 'Macho', 'Encontrado en el parque central', 'Data/usuario/animal/02376ee5-0b04-4a1d-a622-0fa032c397e4.jpg', -33.266110, -66.328357, 'En adopcion'),
(74, 30, 1, 'Misi', '1', 'Gato', 'Pequeño', 0, 'Hembra', 'Vista cerca de la tienda de mascotas', 'Data/usuario/animal/12376ee5-0b04-4a1d-a622-0fa032c397e4.jpg', 40.416700, -3.703700, 'En adopcion'),
(75, 28, 2, 'Bobby', '5', 'Loro', 'Grande', 1, 'Macho', 'Apareció en el vecindario', 'Data/usuario/animal/22376ee5-0b04-4a1d-a622-0fa032c397e4.jpg', 40.417000, -3.704000, 'En recuperacion'),
(76, NULL, 8, 'Luckyy', '2.0', 'Dodo', 'Mediano', 1, 'Macho', 'Encontrado en la plaza', 'Data/usuario/animal/a3e5f80d-933f-48bf-b7f7-4fd02216b190.jpg', 40.418000, -3.705000, 'En recuperacion'),
(78, NULL, 30, NULL, '5.0', 'osito', 'Mediano', 0, 'Macho', 'peligroso', 'Data/usuario/animal/4c7cdb3e-9cc1-41b4-b66b-5dc5b1041325.jpg', -66.297078, -33.295088, 'Reportado');

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

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `noticias`
--

CREATE TABLE `noticias` (
  `Id` int(11) NOT NULL,
  `UsuarioId` int(11) NOT NULL,
  `RefugioId` int(11) NOT NULL,
  `BannerUrl` varchar(255) DEFAULT NULL,
  `Categoria` varchar(255) DEFAULT NULL,
  `Titulo` varchar(255) DEFAULT NULL,
  `Contenido` varchar(1000) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `noticias`
--

INSERT INTO `noticias` (`Id`, `UsuarioId`, `RefugioId`, `BannerUrl`, `Categoria`, `Titulo`, `Contenido`) VALUES
(31, 1, 28, 'Data/usuario/noticia/noticia1.jpg', 'Adopción', 'Nueva campaña de adopción', 'En la búsqueda constante de ofrecer un hogar amoroso a los animales abandonados y maltratados, nuestro refugio de animales está emocionado de anunciar el lanzamiento de una nueva campaña de adopción. Esta iniciativa tiene como objetivo principal sensibilizar a la comunidad sobre la importancia de la adopción y proporcionar a nuestros queridos animales la oportunidad de encontrar familias que les ofrezcan el amor y el cuidado que tanto merecen.'),
(32, 2, 30, 'Data/usuario/noticia/adopcion2.jpg', 'Adopción', 'Adopta un amigo', 'Ven y conoce a los animales que están buscando un hogar lleno de amor.'),
(33, 3, 28, 'Data/usuario/noticia/adopcion3.jpg', 'Adopción', 'Adopción masiva de mascotas', 'Nuestro refugio organiza una jornada de adopción masiva para todos los animales.'),
(34, 1, 30, 'Data/usuario/noticia/adopcion4.jpg', 'Adopción', 'Rescate y adopción', 'Rescatamos y buscamos hogar para los animales más necesitados.'),
(35, 2, 28, 'Data/usuario/noticia/reparacion1.jpg', 'Noticias', 'Actualización sobre el refugio', 'Estamos mejorando nuestras instalaciones para ofrecer un mejor servicio.'),
(37, 1, 28, 'Data/usuario/noticia/acontecimiento2.jpg', 'Acontecimientos', 'Aparece el gatonejo ', 'En un sorprendente giro de los eventos, se ha descubierto una nueva criatura que ha capturado la imaginación de todos: el Conejo Gato. Este fascinante animal, que parece una mezcla perfecta entre un conejo y un gato, fue avistado por primera vez en los bosques de nuestra ciudad.\n\nEl Conejo Gato combina las orejas largas y el hocico del conejo con la agilidad y el pelaje suave de un gato. Los residentes que han tenido la suerte de verlo describen al Conejo Gato como una criatura extremadamente amistosa y juguetona, que salta como un conejo pero ronronea como un gato.'),
(38, 3, 30, 'Data/usuario/noticia/consejo1.jpg', 'Consejos', 'Consejos para el verano', 'Mantén a tus mascotas frescas y seguras durante el verano con estos consejos.'),
(39, 2, 28, 'Data/usuario/noticia/consejo2.jpg', 'Consejos', 'Consejos de salud para mascotas', 'Aprende cómo mantener a tus mascotas saludables y felices.'),
(40, 1, 30, 'Data/usuario/noticia/frio.jpg', 'Consejos', 'Consejos para el invierno', 'Protege a tus mascotas durante los fríos meses de invierno.'),
(41, 8, 30, 'Data/usuario/noticia/noticia2.jpg', 'Acontecimientos', 'Aparece el Nuevo Avatar Perro', 'En un mundo dividido por cuatro reinos caninos - el Reino de los Labradores, la Tribu de los Chihuahuas del Sur, la Nación de los Beagles y los Pastores del Aire - solo un perro puede unirlos a todos: el Avatar Perro. \r\ncon la increíble habilidad de controlar los cuatro elementos del mundo canino: Huesos, Agua Perruna, Fuego Canino y Aire del Parque. Con sus orejas erguidas y su mirada determinada, Luna está listo para embarcarse en aventuras épicas, resolver conflictos entre los reinos caninos y, por supuesto, hacer nuevos amigos peludos en el camino.');

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
(28, 2, 'Guarida de los Peludos', 'Calle Verdadera 3,14', 'En la Guarida de los Peludos, trabajamos incansablemente para convertir cada día en una oportunidad para que nuestros peludos encuentren su familia perfecta', '36524444', -66.328357, -33.266110, 1500, 'Data/usuario/refugio/Refugio1.png'),
(30, 8, 'The Dodo`s last resort ', 'Avenida Renacentista 3,14', 'convirtiendo lo imposible en realidad, los dodos regresan.', '2664878787', -66.357042, -33.319771, 1000, 'Data/usuario/refugio/acbb84f6-b073-466d-8627-57390b42bde0.jpg');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `tareas`
--

CREATE TABLE `tareas` (
  `Id` int(11) NOT NULL,
  `UsuarioId` int(11) DEFAULT NULL,
  `RefugioId` int(11) NOT NULL,
  `Actividad` varchar(150) NOT NULL,
  `Descripcion` varchar(250) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `tareas`
--

INSERT INTO `tareas` (`Id`, `UsuarioId`, `RefugioId`, `Actividad`, `Descripcion`) VALUES
(38, 1, 28, 'Alimentar a los animales', 'Darle el té a las nutrias  a las seis de la tarde.'),
(39, 2, 30, 'Clase de Agilidad', 'Montar una pista de agilidad y entrenar a los perros para mejorar su destreza.'),
(40, 3, 28, 'Sesión de Fotos', 'Hacer una sesión de fotos profesional para las mascotas del refugio y promover su adopción.'),
(41, NULL, 30, 'Maratón de Películas de Animales', 'Proyectar películas de animales y permitir que las mascotas se relajen y disfruten.'),
(42, 8, 28, 'Clases de ingles a los loros', 'Preparar a los loros para su examen de ingles .'),
(43, 30, 30, 'Competencia de Talentos', 'Organizar una competencia donde las mascotas muestren sus trucos y habilidades.');

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
(1, 'María', 'Gomez', '12345678', '26647878', 'Gomez@hotmail.com', 'ipKy7T47aO1Abua2D4f7lyBoJaTBcnE1jdSE0iGhDLI=', 'Data/usuario/avatar_1e6c82e2e-5eda-490a-ad17-71830f2051e5.jpg'),
(2, 'Rufina', 'López', '87654321', '36524444', 'Lopez@hotmail.com', 'ipKy7T47aO1Abua2D4f7lyBoJaTBcnE1jdSE0iGhDLI=', 'Data/usuario/avatar_1e6c82e2e-5eda-490a-ad17-71830f2051e9.jpg'),
(3, 'Albert', 'Tesla', '38439123', '08001010', 'Tesla@hotmail.com', 'ipKy7T47aO1Abua2D4f7lyBoJaTBcnE1jdSE0iGhDLI=', 'Data/usuario/avatar_1e6c82e2e-5eda-490a-ad17-71830f2051e6.jpg'),
(8, 'Marti', 'Panelo', '38383838', '2664878787', 'martin@panelo.com', 'ipKy7T47aO1Abua2D4f7lyBoJaTBcnE1jdSE0iGhDLI=', 'Data/usuario/avatar_825d8925c-3670-4f12-b013-e9c34e1ae660.jpg'),
(30, 'contra', 'contra', '11111124', '222222', 'contra@contra.com', 'ipKy7T47aO1Abua2D4f7lyBoJaTBcnE1jdSE0iGhDLI=', 'Data/usuario/avatar_304882f444-ce15-4392-af16-a82f50bfa7c8.jpg');

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
  ADD KEY `RefugioId` (`RefugioId`),
  ADD KEY `UsuarioId` (`UsuarioId`);

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
  ADD PRIMARY KEY (`Id`),
  ADD KEY `UsuarioId` (`UsuarioId`),
  ADD KEY `RefugioId` (`RefugioId`);

--
-- Indices de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  ADD PRIMARY KEY (`Id`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `animales`
--
ALTER TABLE `animales`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=79;

--
-- AUTO_INCREMENT de la tabla `eventos`
--
ALTER TABLE `eventos`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT de la tabla `noticias`
--
ALTER TABLE `noticias`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=42;

--
-- AUTO_INCREMENT de la tabla `refugios`
--
ALTER TABLE `refugios`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=31;

--
-- AUTO_INCREMENT de la tabla `tareas`
--
ALTER TABLE `tareas`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=44;

--
-- AUTO_INCREMENT de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=31;

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
  ADD CONSTRAINT `noticias_ibfk_1` FOREIGN KEY (`UsuarioId`) REFERENCES `usuarios` (`Id`) ON DELETE CASCADE,
  ADD CONSTRAINT `noticias_ibfk_2` FOREIGN KEY (`RefugioId`) REFERENCES `refugios` (`Id`) ON DELETE CASCADE;

--
-- Filtros para la tabla `refugios`
--
ALTER TABLE `refugios`
  ADD CONSTRAINT `refugio_ibfk_1` FOREIGN KEY (`UsuarioId`) REFERENCES `usuarios` (`Id`) ON DELETE CASCADE;

--
-- Filtros para la tabla `tareas`
--
ALTER TABLE `tareas`
  ADD CONSTRAINT `tareas_ibfk_1` FOREIGN KEY (`UsuarioId`) REFERENCES `usuarios` (`Id`) ON DELETE CASCADE,
  ADD CONSTRAINT `tareas_ibfk_2` FOREIGN KEY (`RefugioId`) REFERENCES `refugios` (`Id`) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
