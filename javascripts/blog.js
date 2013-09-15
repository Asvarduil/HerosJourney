var $accordion = $('#blog-accordion');

$(document).ready(function () {
	var blogAccordion = new AsvarduilAccordion($accordion);
	blogAccordion.animateRate = 200;
	blogAccordion.Activate();
});